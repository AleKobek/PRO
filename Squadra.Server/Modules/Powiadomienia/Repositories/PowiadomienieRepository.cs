using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Powiadomienia.DTO;
using Squadra.Server.Modules.Powiadomienia.Enums;
using Squadra.Server.Modules.Powiadomienia.Models;

namespace Squadra.Server.Modules.Powiadomienia.Repositories;

public class PowiadomienieRepository(AppDbContext context) : IPowiadomienieRepository
{
    public static readonly int MaksymalnaLiczbaPowiadomien = 250;
    
    public async Task<Powiadomienie> GetPowiadomienie(int id)
    {
        var powiadomienie = await context.Powiadomienie.FindAsync(id);
        if(powiadomienie == null) throw new NieZnalezionoWBazieException("Powiadomienie o id " + id + " nie istnieje");
        var typPowiadomienia = await context.TypPowiadomienia.FindAsync(powiadomienie.TypPowiadomieniaId);
        if(typPowiadomienia == null) throw new NieZnalezionoWBazieException("Typ powiadomienia o id " + id + " nie istnieje");

        return powiadomienie;
    }

    public async Task<ICollection<Powiadomienie>> GetPowiadomieniaUzytkownika(int idUzytkownika)
    {
        var powiadomienia = await context.Powiadomienie.Where(x => x.UzytkownikId == idUzytkownika).ToListAsync();
        powiadomienia.Sort((x, y) => y.DataWyslania.CompareTo(x.DataWyslania));
        var lista = new List<Powiadomienie>();
        foreach (var powiadomienie in powiadomienia) 
            lista.Add(await GetPowiadomienie(powiadomienie.Id));
        return lista;
    }
    
    public async Task<ICollection<Powiadomienie>> GetZaproszeniaDoDruzynyUzytkownika(int idUzytkownika)
    {
        var powiadomienia = await context.Powiadomienie
            .Where(x => x.UzytkownikId == idUzytkownika && x.TypPowiadomieniaId == (int)TypPowiadomieniaEnum.ZaproszenieDoDruzyny)
            .ToListAsync();
        powiadomienia.Sort((x, y) => y.DataWyslania.CompareTo(x.DataWyslania));
        var lista = new List<Powiadomienie>();
        foreach (var powiadomienie in powiadomienia) 
            lista.Add(await GetPowiadomienie(powiadomienie.Id));
        return lista;
    }
    
    public async Task<ICollection<Powiadomienie>> PodajPowiadomieniaUzytkownikaPrzekraczajaceLimit(int idUzytkownika)
    {
        var liczbaPowiadomien = await context.Powiadomienie.CountAsync(x => x.UzytkownikId == idUzytkownika);
        if(liczbaPowiadomien <= MaksymalnaLiczbaPowiadomien) return new List<Powiadomienie>();
        var powiadomieniaDoUsuniecia = await context.Powiadomienie
            .Where(x => x.UzytkownikId == idUzytkownika)
            .OrderBy(x => x.DataWyslania)
            .Take(liczbaPowiadomien - MaksymalnaLiczbaPowiadomien)
            .ToListAsync();
        return powiadomieniaDoUsuniecia;
    }

    public async Task<bool> CzyUzytkownikMaPowiadomienieDanegoTypuPowiazaneZObiektami(int idUzytkownika, int idTypu, int idPowiazanegoObiektu, int? idDrugiegoPowiazanegoObiektu)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if(uzytkownik == null) throw new NieZnalezionoWBazieException("Użytkownik o id " + idUzytkownika + " nie istnieje");
        
        return await context.Powiadomienie.AnyAsync(x => 
            x.UzytkownikId == idUzytkownika 
            && x.TypPowiadomieniaId == idTypu
            && x.PowiazanyObiektId == idPowiazanegoObiektu
            && (idDrugiegoPowiazanegoObiektu == null || x.DrugiPowiazanyObiektId == idDrugiegoPowiazanegoObiektu)
        );
    }

    public async Task<bool> CreatePowiadomienie(PowiadomienieCreateDto powiadomienie)
    {
        var powiadomienieDoDodania = new Powiadomienie
        {
            DataWyslania = DateTime.Now, // liczymy moment, w którym dotrze do bazy
            PowiazanyObiektId = powiadomienie.IdPowiazanegoObiektu,
            DrugiPowiazanyObiektId = powiadomienie.IdDrugiegoPowiazanegoObiektu,
            Tresc = powiadomienie.Tresc,
            TypPowiadomieniaId = powiadomienie.IdTypuPowiadomienia,
            PowiazanyObiektNazwa = powiadomienie.NazwaPowiazanegoObiektu,
            DrugiPowiazanyObiektNazwa = powiadomienie.NazwaDrugiegoPowiazanegoObiektu,
            UzytkownikId = powiadomienie.IdUzytkownika
        };
        await context.Powiadomienie.AddAsync(powiadomienieDoDodania);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePowiadomienie(int id)
    {
        var powiadomienie = await context.Powiadomienie.FindAsync(id);
        if(id == 1) throw new NieZnalezionoWBazieException("Nie można usunąć powiadomienia Ello >:C");
        if(powiadomienie == null) throw new NieZnalezionoWBazieException("Powiadomienie o id " + id + " nie istnieje");;
        context.Powiadomienie.Remove(powiadomienie);
        await context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> UsunPowiadomienia(ICollection<Powiadomienie> powiadomienia)
    {
        context.Powiadomienie.RemoveRange(powiadomienia);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePowiadomieniaZwiazaneZUzytkownikiem(int idUzytkownika)
    {
        var powiadomienia = await context.Powiadomienie.Where(x => 
            x.UzytkownikId == idUzytkownika 
            || x.PowiazanyObiektId == idUzytkownika
            || x.DrugiPowiazanyObiektId == idUzytkownika
        ).ToListAsync();
        context.Powiadomienie.RemoveRange(powiadomienia);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UsunPowiadomieniaZwiazaneZDruzyna(int idDruzyny)
    {
        var powiadomienia = await context.Powiadomienie
            .Where(x => 
                x.PowiazanyObiektId == idDruzyny
                || x.DrugiPowiazanyObiektId == idDruzyny
            )
            .ToListAsync();
        context.Powiadomienie.RemoveRange(powiadomienia);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePowiadomieniaDanegoTypuPowiazaneZObiektami(int? idUzytkownika,int idTypu, int idPowiazanegoObiektu, int? idDrugiegoPowiazanegoObiektu)
    {
        var powiadomienia = await context.Powiadomienie
            .Where(x => 
                x.TypPowiadomieniaId == idTypu 
                && x.PowiazanyObiektId == idPowiazanegoObiektu
                && (idDrugiegoPowiazanegoObiektu == null || x.DrugiPowiazanyObiektId == idDrugiegoPowiazanegoObiektu)
                && (idUzytkownika == null || x.UzytkownikId == idUzytkownika)
                )
            .ToListAsync();
        context.Powiadomienie.RemoveRange(powiadomienia);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<string> GetNazwaTypuPowiadomienia(int idTypuPowiadomienia)
    {
        var typPowiadomienia = await context.TypPowiadomienia.FindAsync(idTypuPowiadomienia);
        if(typPowiadomienia == null) throw new NieZnalezionoWBazieException("Typ powiadomienia o id " + idTypuPowiadomienia + " nie istnieje");
        return typPowiadomienia.Nazwa;
    }
}