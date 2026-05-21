using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Drużyny.DTO;
using Squadra.Server.Modules.Drużyny.Models;
using Squadra.Server.Modules.Statystyki.Models;
using Squadra.Server.Modules.Statystyki.Repositories;

namespace Squadra.Server.Modules.Drużyny.Repositories;

public class DruzynyRepository(AppDbContext context, IStatystykiRepository statystykiRepository) : IDruzynyRepository
{
    
    public async Task<Druzyna> GetDruzyna(int idDruzyny)
    {
        var druzyna = await context.Druzyna.FindAsync(idDruzyny);
        if (druzyna == null) throw new NieZnalezionoWBazieException("Nie znaleziono drużyny o id " + idDruzyny);
        return druzyna;
    }
    
    public async Task<ICollection<Druzyna>> GetDruzynyUzytkownika(int idUzytkownika)
    {
        var miejscaWDruzynie = await context.MiejsceWDruzynie
            .Where(m => m.UzytkownikId == idUzytkownika)
            .Include(m => m.Druzyna)
            .ToListAsync();

        return miejscaWDruzynie.Select(m => m.Druzyna).ToList();
    }
    
    public async Task<ICollection<MiejsceWDruzynie>> GetMiejscaWDruzynie(int idDruzyny)
    {
        var druzyna = await context.Druzyna.FindAsync(idDruzyny);
        if (druzyna == null) throw new NieZnalezionoWBazieException("Nie znaleziono drużyny o id " + idDruzyny);
        
        var miejscaWDruzynie = await context.MiejsceWDruzynie
            .Where(m => m.DruzynaId == idDruzyny)
            .Include(m => m.Rola)
            .ToListAsync();

        return miejscaWDruzynie;
    }
    
    public async Task<ICollection<NastrojRozgrywki>> GetNastrojeRozgrywki()
    {
        return await context.NastrojRozgrywki.ToListAsync();
    }
    
    public async Task<NastrojRozgrywki> GetNastrojRozgrywki(int idNastroju)
    {
        var nastroj = await context.NastrojRozgrywki.FindAsync(idNastroju);
        if (nastroj == null) throw new NieZnalezionoWBazieException("Nie znaleziono nastroju o id " + idNastroju);
        return nastroj;
    }

    // w serwisie trzeba będzie posprawdzać id wszystkich elementów czy istnieje
    public async Task<bool> StworzDruzyne(CreateDruzynaReqDto druzynaReq, int idKapitana)
    {
        var nastrojRozgrywki = await context.NastrojRozgrywki.FindAsync(druzynaReq.IdNastrojuRozgrywki);
        if (nastrojRozgrywki == null) throw new NieZnalezionoWBazieException("Nie znaleziono nastroju o id " + druzynaReq.IdNastrojuRozgrywki);
        
        var nieprawidloweIdRol = druzynaReq.MiejscaWDruzynie.Select(x => x.IdRoli)
                .Where(x => !context.Rola.Any(r => r.Id == x)).ToList();
            if (nieprawidloweIdRol.Count > 0)
                throw new NieZnalezionoWBazieException("Nie znaleziono róli o id: " + string.Join(", ", nieprawidloweIdRol));
        
        var transakcja = await context.Database.BeginTransactionAsync();
        try
        {
            var czyMaWymagania = druzynaReq.WymaganeStatystyki is { Count: > 0 } 
                                 || druzynaReq.MiejscaWDruzynie
                                     .Any(x => x.WymaganaStatystyka is not null);
            
            var druzyna = new Druzyna
            {
                Nazwa = druzynaReq.Nazwa,
                GraId = druzynaReq.IdGry,
                KapitanId = idKapitana,
                CzyPubliczna = druzynaReq.CzyPubliczna,
                Opis = druzynaReq.Opis,
                NastrojRozgrywkiId = druzynaReq.IdNastrojuRozgrywki,
                WymaganyJezykId = druzynaReq.IdWymaganegoJezyka,
                WymaganyStopienBieglosciJezykaId = druzynaReq.IdWymaganegoStopniaBieglosciJezyka,
                Czy18Plus = druzynaReq.Czy18Plus,
                PlatformaId = druzynaReq.IdPlatformy,
                CzyMaWymagania = czyMaWymagania
            };
            // dodajemy drużynę i bierzemy jej id, żeby potem dodać wymagania i miejsca
            var dodanaDruzyna = context.Druzyna.Add(druzyna);
            await context.SaveChangesAsync();
            var idDruzyny = dodanaDruzyna.Entity.Id;
            if(druzynaReq.WymaganeStatystyki != null && druzynaReq.WymaganeStatystyki.Count > 0)
            {
                foreach (var wymaganaStatystyka in druzynaReq.WymaganeStatystyki)
                {
                    var nowaWymaganaStatystykaDruzyny = new WymaganaStatystykaDruzyny
                    {
                        DruzynaId = idDruzyny,
                        StatystykaId = wymaganaStatystyka.IdStatystyki,
                        Wartosc = wymaganaStatystyka.Wartosc,
                        PorownywalnaWartoscLiczbowa = wymaganaStatystyka.PorownywalnaWartoscLiczbowa
                    };
                    context.WymaganaStatystykaDruzyny.Add(nowaWymaganaStatystykaDruzyny);
                }
            }
            foreach (var miejsce in druzynaReq.MiejscaWDruzynie)
            {
                var noweMiejsceWDruzynie = new MiejsceWDruzynie
                {
                    DruzynaId = idDruzyny,
                    UzytkownikId = idKapitana,
                    RolaId = miejsce.IdRoli,
                    StatystykaId = miejsce.WymaganaStatystyka?.IdStatystyki,
                    WartoscStatystyki = miejsce.WymaganaStatystyka?.Wartosc,
                    WartoscLiczbowaStatystyki = miejsce.WymaganaStatystyka?.PorownywalnaWartoscLiczbowa
                };
                context.MiejsceWDruzynie.Add(noweMiejsceWDruzynie);
            }
            await context.SaveChangesAsync();
            await transakcja.CommitAsync();
            return true;
        }
        catch (Exception e)
        {
            await transakcja.RollbackAsync();
            Console.WriteLine("Wystąpił błąd podczas tworzenia drużyny: " + e.Message);
            return false;
        }
    }
    
    public async Task<bool> UsunDruzyne(int idDruzyny)
    {
        // jeśli istnieje zewnętrzna transakcja, użyjemy jej; w przeciwnym razie utworzymy nową
        IDbContextTransaction? transakcja = context.Database.CurrentTransaction;
        var createdTransaction = false;
        if (transakcja == null)
        {
            transakcja = await context.Database.BeginTransactionAsync();
            createdTransaction = true;
        }

        var druzyna = await context.Druzyna.FindAsync(idDruzyny);
        if (druzyna == null) throw new NieZnalezionoWBazieException("Nie znaleziono drużyny o id " + idDruzyny);
        try
        {
            // usuwamy wymagane statystyki drużyny
            await statystykiRepository.UsunWymaganeStatystykiDruzyny(idDruzyny);
            // usuwamy wszystkie miejsca w drużynie
            var miejscaWDruzynie = await context.MiejsceWDruzynie.Where(m => m.DruzynaId == idDruzyny).ToListAsync();
            context.MiejsceWDruzynie.RemoveRange(miejscaWDruzynie);
            // usuwamy drużynę
            context.Druzyna.Remove(druzyna);
            await context.SaveChangesAsync();

            if (createdTransaction)
            {
                await transakcja.CommitAsync();
            }

            return true;
        }
        catch (Exception e)
        {
            if (createdTransaction)
            {
                await transakcja.RollbackAsync();
                Console.WriteLine("Wystąpił błąd podczas usuwania drużyny: " + e.Message);
                return false;
            }
            // jeśli nie utworzyliśmy transakcji tutaj, rzucamy wyjątek dalej aby zewnętrzna transakcja mogła go obsłużyć/rollbackować
            throw;
        }
    }
    
    public async Task<bool> OpuscDruzyne(int idDruzyny, int idUzytkownika)
    {
        var miejsceWDruzynie = await context.MiejsceWDruzynie.FirstOrDefaultAsync(m => m.DruzynaId == idDruzyny && m.UzytkownikId == idUzytkownika);
        if (miejsceWDruzynie == null) throw new NieZnalezionoWBazieException("Nie znaleziono miejsca w drużynie dla użytkownika o id " + idUzytkownika + " w drużynie o id " + idDruzyny);
        miejsceWDruzynie.UzytkownikId = null;
        await context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> WyrzucUzytkownikaZeWszystkichDruzyn(int idUzytkownika)
    {
        var miejscaWDruzynie = await context.MiejsceWDruzynie.Where(m => m.UzytkownikId == idUzytkownika).ToListAsync();
        var zaktualiZowaneMiejsca = miejscaWDruzynie.Select(x => { x.UzytkownikId = null; return x;}).ToList();
        context.MiejsceWDruzynie.UpdateRange(zaktualiZowaneMiejsca);
        await context.SaveChangesAsync();
        return true;
    }
}
