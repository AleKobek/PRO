using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.DTO.Powiadomienie;
using Squadra.Server.Exceptions;
using Squadra.Server.Models;

namespace Squadra.Server.Repositories;

public class PowiadomienieRepository(AppDbContext context, IProfilRepository profilRepository) : IPowiadomienieRepository
{
    public async Task<PowiadomienieDto> GetPowiadomienie(int id)
    {
        var powiadomienie = await context.Powiadomienie.FindAsync(id);
        if(powiadomienie == null) throw new NieZnalezionoWBazieException("Powiadomienie o id " + id + " nie istnieje");
        var typPowiadomienia = await context.TypPowiadomienia.FindAsync(powiadomienie.TypPowiadomieniaId);
        if(typPowiadomienia == null) throw new NieZnalezionoWBazieException("Typ powiadomienia o id " + id + " nie istnieje");
        if (powiadomienie.PowiazanyObiektId != null && 
            (powiadomienie.TypPowiadomieniaId is 2 or 4)) {
            // te dwie linijki pod spodem robimy tylko po to, aby kompilator nie płakał
            var idPowiazanegoUzytkownika = powiadomienie.PowiazanyObiektId ?? -1;
            if (idPowiazanegoUzytkownika == -1) throw new NieZnalezionoWBazieException("Użytkownik o takim id nie istnieje");
            var powiazanyProfil = await profilRepository.GetProfilUzytkownika(idPowiazanegoUzytkownika);
            return new PowiadomienieDto(
                powiadomienie.Id,
                powiadomienie.TypPowiadomieniaId,
                powiadomienie.UzytkownikId,
                idPowiazanegoUzytkownika,
                powiazanyProfil.Pseudonim,
                powiadomienie.Tresc,
                powiadomienie.DataWyslania.ToString("dd.MM.yyyy HH:mm")
            );
        }
        // jak tu dochodzimy, to jest systemowe
        return new PowiadomienieDto(
            powiadomienie.Id,
            powiadomienie.TypPowiadomieniaId,
            powiadomienie.UzytkownikId,
            null,
            null,
            powiadomienie.Tresc,
            powiadomienie.DataWyslania.ToString("dd.MM.yyyy HH:mm")
        );
    }

    public async Task<ICollection<PowiadomienieDto>> GetPowiadomieniaUzytkownika(int idUzytkownika)
    {
        var powiadomienia = await context.Powiadomienie.Where(x => x.UzytkownikId == idUzytkownika).ToListAsync();
        powiadomienia.Sort((x, y) => y.DataWyslania.CompareTo(x.DataWyslania));
        var lista = new List<PowiadomienieDto>();
        foreach (var powiadomienie in powiadomienia) 
            lista.Add(await GetPowiadomienie(powiadomienie.Id));
        return lista;
    }

    public async Task<bool> CreatePowiadomienie(PowiadomienieCreateDto powiadomienie)
    {
        var powiadomienieDoDodania = new Powiadomienie
        {
            DataWyslania = DateTime.Now, // liczymy moment, w którym dotrze do bazy
            PowiazanyObiektId = powiadomienie.IdPowiazanegoObiektu,
            Tresc = powiadomienie.Tresc,
            TypPowiadomieniaId = powiadomienie.IdTypuPowiadomienia,
            PowiazanyObiektNazwa = powiadomienie.NazwaPowiazanegoObiektu,
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
    
    public async Task<bool> DeletePowiadomieniaUzytkownika(int idUzytkownika)
    {
        var powiadomienia = await context.Powiadomienie.Where(x => x.UzytkownikId == idUzytkownika).ToListAsync();
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
    
    // w service będzie stwórz powiadomienie każdego typu, na razie mamy:
    //  - systemowe,
    //  - zaproszenie do znajomych,
    //  - przyjęto zaproszenie do znajomych
    //  - odrzucono zaproszenie do znajomych,
}