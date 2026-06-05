using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Drużyny.DTO;
using Squadra.Server.Modules.Drużyny.Models;
using Squadra.Server.Modules.Platformy;
using Squadra.Server.Modules.Statystyki.Models;
using Squadra.Server.Modules.Statystyki.Repositories;

namespace Squadra.Server.Modules.Drużyny.Repositories;

public class DruzynyRepository(AppDbContext context, IStatystykiRepository statystykiRepository) : IDruzynyRepository
{
    public static readonly int MaksymalnaLiczbaDruzynGraczaDlaGry = 10;
    
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

        return miejscaWDruzynie.Select(m => m.Druzyna).Distinct().ToList();
    }
    
    public async Task<ICollection<Druzyna>> GetDruzyny(int[] idDruzyn)
    {
        var druzyny = await context.Druzyna
            .Where(d => idDruzyn.Contains(d.Id))
            .ToListAsync();

        return druzyny;
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
    
    public async Task<MiejsceWDruzynie> GetMiejsceWDruzynie(int idMiejsca)
    {
        var miejsce = await context.MiejsceWDruzynie
            .Include(m => m.Rola)
            .FirstOrDefaultAsync(m => m.Id == idMiejsca);
        if (miejsce == null) throw new NieZnalezionoWBazieException("Nie znaleziono miejsca w drużynie o id " + idMiejsca);
        return miejsce;
    }
    
    public async Task<int> GetIdKapitanaDruzynyMiejsca(int idMiejsca)
    {
        var miejsce = await context.MiejsceWDruzynie.FindAsync(idMiejsca);
        if (miejsce == null) throw new NieZnalezionoWBazieException("Nie znaleziono miejsca w drużynie o id " + idMiejsca);
        
        var druzyna = await context.Druzyna.FindAsync(miejsce.DruzynaId);
        if (druzyna == null) throw new NieZnalezionoWBazieException("Nie znaleziono drużyny o id " + miejsce.DruzynaId);
        
        return druzyna.KapitanId;
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

    public async Task<bool> CzyUzytkownikSpelniaWymaganieMiejsca(int idMiejsca, int idUzytkownika)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik == null) throw new NieZnalezionoWBazieException("Uzytkownik o id " + idUzytkownika + " nie istnieje.");

        var miejsce = await context.MiejsceWDruzynie.FindAsync(idMiejsca);
        if (miejsce == null) throw new NieZnalezionoWBazieException("Nie znaleziono miejsca w drużynie o id " + idMiejsca);

        if (miejsce.StatystykaId == null) return true; // jeżeli nie ma wymaganej statystyki, to każdy spełnia wymaganie
        
        // jeżeli jest wymagana statystyka, sprawdzamy czy użytkownik ma tę statystykę i czy spełnia wymaganie
        var statystykaUzytkownika = await context.StatystykaUzytkownika
            .FirstOrDefaultAsync(s => s.UzytkownikId == idUzytkownika && s.StatystykaId == miejsce.StatystykaId);
        if (statystykaUzytkownika == null) return false; // jeśli użytkownik nie ma tej statystyki, to nie spełnia wymagania
            
        if (miejsce.WartoscLiczbowaStatystyki != null)
        {
            return statystykaUzytkownika.PorownywalnaWartoscLiczbowa >= miejsce.WartoscLiczbowaStatystyki;
        }
            
        return statystykaUzytkownika.Wartosc == miejsce.WartoscStatystyki;

    }

    public async Task<bool> CzyUzytkownikSpelniaWymaganiaDruzyny(int idDruzyny, int idUzytkownika)
    {
        var wymaganiaDruzyny = await context.WymaganaStatystykaDruzyny
            .Where(m => m.DruzynaId == idDruzyny)
            .ToListAsync();
        // przechodzimy po kolei przez wymagania i sprawdzamy czy użytkownik je spełnia, jeśli któreś nie jest spełnione, to zwracamy false
        foreach (var wymaganie in wymaganiaDruzyny)
        {
            var statystykaUzytkownika = await context.StatystykaUzytkownika
                .FirstOrDefaultAsync(s => s.UzytkownikId == idUzytkownika && s.StatystykaId == wymaganie.StatystykaId);
            if (statystykaUzytkownika == null) return false; // jeśli użytkownik nie ma tej statystyki, to nie spełnia wymagania
            
            if (wymaganie.PorownywalnaWartoscLiczbowa != null)
            {
                if(statystykaUzytkownika.PorownywalnaWartoscLiczbowa < wymaganie.PorownywalnaWartoscLiczbowa) return false;
            }
            else
            {
                if (statystykaUzytkownika.Wartosc != wymaganie.Wartosc) return false;
            }
        }
        return true; // jeśli spełnia wszystkie wymagania, to zwracamy true
    }

    public async Task<bool> CzyUzytkownikPrzekraczaMaksLiczbeDruzyn(int idUzytkownika, int idGry)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik == null) throw new NieZnalezionoWBazieException("Uzytkownik o id " + idUzytkownika + " nie istnieje.");
        
        var gra = await context.WspieranaGra.FindAsync(idGry);
        if (gra == null) throw new NieZnalezionoWBazieException("Nie znaleziono gry o id " + idGry);
        
        // Liczba drużyn, w których użytkownik jest członkiem (ma przypisany UzytkownikId) i drużyna dotyczy danej gry
        var liczbaDruzynGracza = await context.Druzyna
            .Where(d => d.GraId == idGry && d.MiejsceWDruzynieCollection.Any(m => m.UzytkownikId == idUzytkownika))
            .CountAsync();
        
        Console.WriteLine("####################################################");
        Console.WriteLine($"Liczba drużyn użytkownika {idUzytkownika} w grze {idGry}: {liczbaDruzynGracza}");
        
        return liczbaDruzynGracza >= MaksymalnaLiczbaDruzynGraczaDlaGry;
    }
    
    public async Task<bool> StworzDruzyne(CreateDruzynaReqDto druzynaReq, int idKapitana)
    {
        var nastrojRozgrywki = await context.NastrojRozgrywki.FindAsync(druzynaReq.IdNastrojuRozgrywki);
        if (nastrojRozgrywki == null) throw new NieZnalezionoWBazieException("Nie znaleziono nastroju o id " + druzynaReq.IdNastrojuRozgrywki);
        
        var nieprawidloweIdRol = druzynaReq.MiejscaWDruzynie.Select(x => x.IdRoli)
                .Where(x => x != null && !context.Rola.Any(r => r.Id == x)).ToList();
            if (nieprawidloweIdRol.Count > 0)
                throw new NieZnalezionoWBazieException("Nie znaleziono roli o id: " + string.Join(", ", nieprawidloweIdRol));
        
        var transakcja = await context.Database.BeginTransactionAsync();
        try
        {
            
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
                PlatformaId = druzynaReq.IdPlatformy,
                CzyZintegrowano = druzynaReq.CzyZintegrowana
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
            // dodajemy miejsce kapitana
            var miejsceKapitana = new MiejsceWDruzynie
            {
                DruzynaId = idDruzyny,
                UzytkownikId = idKapitana,
                RolaId = druzynaReq.IdRoliKapitana,
                StatystykaId = null,
                WartoscStatystyki = null,
                WartoscLiczbowaStatystyki = null
            };
            context.MiejsceWDruzynie.Add(miejsceKapitana);
            // dodajemy pozostałe miejsca
            foreach (var miejsce in druzynaReq.MiejscaWDruzynie)
            {
                var noweMiejsceWDruzynie = new MiejsceWDruzynie
                {
                    DruzynaId = idDruzyny,
                    UzytkownikId = null,
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
    
    public async Task<bool> OproznijMiejsceWDruzynie(int idMiejsca)
    {
        var miejsceWDruzynie = await context.MiejsceWDruzynie.FindAsync(idMiejsca);
        if (miejsceWDruzynie == null) throw new NieZnalezionoWBazieException("Nie znaleziono miejsca w drużynie o id " + idMiejsca);
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
    
    public async Task<bool> WyrzucUzytkownikaZeWszystkichZintegrowanychDruzyn(int idUzytkownika)
    {
        var miejscaWDruzynie = await context.MiejsceWDruzynie
            .Include(x => x.Druzyna)
            .Where(m => m.UzytkownikId == idUzytkownika && m.Druzyna.CzyZintegrowano).ToListAsync();
        var zaktualiZowaneMiejsca = miejscaWDruzynie.Select(x => { x.UzytkownikId = null; return x;}).ToList();
        context.MiejsceWDruzynie.UpdateRange(zaktualiZowaneMiejsca);
        await context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> UsunWszystkieDruzynyUzytkownika(int idUzytkownika)
    {
        var druzyny = await context.Druzyna.Where(d => d.KapitanId == idUzytkownika).ToListAsync();
        foreach (var druzyna in druzyny)
        {
            var czyUsunieto = await UsunDruzyne(druzyna.Id);
            if (!czyUsunieto) return false;
        }
        return true;
    }
    
    public async Task<bool> UsunWszystkieZintegrowaneDruzynyUzytkownika(int idUzytkownika)
    {
        var druzyny = await context.Druzyna.Where(d => d.KapitanId == idUzytkownika && d.CzyZintegrowano).ToListAsync();
        foreach (var druzyna in druzyny)
        {
            var czyUsunieto = await UsunDruzyne(druzyna.Id);
            if (!czyUsunieto) return false;
        }
        return true;
    }

    public async Task<bool> UpdateDruzyna(int idDruzyny, DruzynaUpdateDto druzynaReq)
    {
        var druzynaDoZmiany = await context.Druzyna.FindAsync(idDruzyny);
        if (druzynaDoZmiany == null) throw new NieZnalezionoWBazieException("Nie znaleziono drużyny o id " + idDruzyny);

        druzynaDoZmiany.Nazwa = druzynaReq.Nazwa.Trim();
        druzynaDoZmiany.CzyPubliczna = druzynaReq.CzyPubliczna;
        druzynaDoZmiany.Opis = druzynaReq.Opis?.Trim();
        druzynaDoZmiany.NastrojRozgrywkiId = druzynaReq.IdNastrojuRozgrywki;
        await context.SaveChangesAsync();

        return true;
    }
    
    public async Task<ICollection<int>> WyszukajIdDruzyn(WyszukajDruzyneReqDto req, int idUzytkownika)
    {
        var druzyny = await context.Druzyna
            .Where(d => d.GraId == req.IdGry
                        && (req.IdPlatformy == null || d.PlatformaId == req.IdPlatformy)
                        && (req.IdNastrojuRozgrywki == null || d.NastrojRozgrywkiId == req.IdNastrojuRozgrywki)
                        && (req.IdJezyka == null || d.WymaganyJezykId == req.IdJezyka)
                        && (req.IdStopnia == null || d.WymaganyStopienBieglosciJezykaId == req.IdStopnia)
                        && d.CzyZintegrowano == req.CzyZintegrowano
                        && (string.IsNullOrEmpty(req.Nazwa) || d.Nazwa.Contains(req.Nazwa.Trim()))
                        && d.MiejsceWDruzynieCollection.All(m => m.UzytkownikId != idUzytkownika)
            )
            .Include(d => d.MiejsceWDruzynieCollection)
            .ToListAsync();

        // jeżeli nie ma wymagań, obchodzą nas tylko role i wolne miejsca
        if(!req.CzyZintegrowano)
        {
            // na górze odfiltrowujemy tak, aby zostało bez ról tylko wtedy gdy faktycznie nie ma ról
            if (req.IdRol.Length == 0)
            {
                // po prostu ma mieć wolne miejsca
                druzyny = druzyny.Where(d => d.MiejsceWDruzynieCollection.Any(m => m.UzytkownikId == null)).ToList();
            }
            else
            {
                druzyny = druzyny.Where(d => d.MiejsceWDruzynieCollection
                    .Any(m =>
                        m.UzytkownikId == null // jest wolne
                        && ( // rola nam pasuje
                            m.RolaId == null
                            || req.IdRol.Contains(m.RolaId.Value)
                        )
                    )
                ).ToList();
            }

            return druzyny.Select(x => x.Id).ToList();
        }
        // jeżeli mogą być wymagania
        List<int> przefiltrowaneDruzyny = [];
        foreach (var druzyna in druzyny)
        {
            if (!await CzyUzytkownikSpelniaWymaganiaDruzyny(druzyna.Id, idUzytkownika)) continue;
            
            var miejscaWDruzynie = druzyna.MiejsceWDruzynieCollection.Where(m => m.UzytkownikId == null).ToList();
            
            // jeżeli są preferencje ról, to filtrujemy miejsca w drużynie, żeby zostały tylko te, które pasują do preferencji ról (albo nie mają przypisanej roli)
            if (req.IdRol.Length > 0) miejscaWDruzynie = miejscaWDruzynie.Where(m => m.RolaId == null || req.IdRol.Contains(m.RolaId.Value)).ToList();
            
            var czyJestMiejsce = false;
            foreach (var miejsce in miejscaWDruzynie)
            {
                if (!await CzyUzytkownikSpelniaWymaganieMiejsca(miejsce.Id, idUzytkownika)) continue;
                // jeżeli spełnia wymaganie
                czyJestMiejsce = true;
                break;
            }
            // są wolne miejsca, na które możemy dołączyć
            if(czyJestMiejsce) przefiltrowaneDruzyny.Add(druzyna.Id);
        }

        return przefiltrowaneDruzyny;
    }
}
