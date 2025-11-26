using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.DTO.Powiadomienie;
using Squadra.Server.Exceptions;
using Squadra.Server.Models;

namespace Squadra.Server.Repositories;

public class PowiadomienieRepository(AppDbContext context, UzytkownikRepository uzytkownikRepository) : IPowiadomienieRepository
{
    public async Task<PowiadomienieDto> GetPowiadomienie(int id)
    {
        var powiadomienie = await context.Powiadomienie.FindAsync(id);
        if(powiadomienie == null) throw new NieZnalezionoWBazieException("Powiadomienie o id " + id + " nie istnieje");
        var typPowiadomienia = await context.TypPowiadomienia.FindAsync(powiadomienie.TypPowiadomieniaId);
        if(typPowiadomienia == null) throw new NieZnalezionoWBazieException("Typ powiadomienia o id " + id + " nie istnieje");
        if (powiadomienie.PowiazanyObiektId != null && 
            (typPowiadomienia.Nazwa == "Zaproszenie do znajomych" || typPowiadomienia.Nazwa =="Odrzucone zaproszenie do znajomych")) {
            // te dwie linijki pod spodem robimy tylko po to, aby kompilator nie płakał
            var idUzytkownika = powiadomienie.PowiazanyObiektId ?? -1;
            if (idUzytkownika == -1) throw new NieZnalezionoWBazieException("Użytkownik o takim id nie istnieje!");
            var uzytkownik = await uzytkownikRepository.GetUzytkownik(idUzytkownika);
            return new PowiadomienieDto(
                powiadomienie.Id,
                typPowiadomienia.Nazwa,
                powiadomienie.UzytkownikId,
                uzytkownik.Id,
                uzytkownik.Login,
                powiadomienie.Tresc,
                powiadomienie.DataWyslania
            );
        }
        // jak tu dochodzimy, to jest systemowe
        return new PowiadomienieDto(
            powiadomienie.Id,
            typPowiadomienia.Nazwa,
            powiadomienie.UzytkownikId,
            null,
            null,
            powiadomienie.Tresc,
            powiadomienie.DataWyslania
        );
    }

    public async Task<ICollection<PowiadomienieDto>> GetPowiadomieniaUzytkownika(int idUzytkownika)
    {
        var powiadomienia = await context.Powiadomienie.Where(x => x.UzytkownikId == idUzytkownika).ToListAsync();
        var listaDoZwrocenia = new List<PowiadomienieDto>();
        foreach (var powiadomienie in powiadomienia) 
            listaDoZwrocenia.Add(await GetPowiadomienie(powiadomienie.Id));
        return listaDoZwrocenia;
    }

    public async Task<bool> CreatePowiadomienie(PowiadomienieCreateDto powiadomienie)
    {
        var powiadomienieDoDodania = new Powiadomienie
        {
            DataWyslania = DateTime.Now, // liczymy moment, w którym dotrze do bazy
            PowiazanyObiektId = powiadomienie.IdPowiazanegoObiektu,
            Tresc = powiadomienie.Tresc,
            TypPowiadomieniaId = powiadomienie.IdTypuPowiadomienia,
            UzytkownikId = powiadomienie.IdUzytkownika
        };
        await context.Powiadomienie.AddAsync(powiadomienieDoDodania);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePowiadomienie(int id)
    {
        var powiadomienie = await context.Powiadomienie.FindAsync(id);
        if(powiadomienie == null) throw new NieZnalezionoWBazieException("Powiadomienie o id " + id + " nie istnieje");;
        context.Powiadomienie.Remove(powiadomienie);
        await context.SaveChangesAsync();
        return true;
    }
    
    // w service będzie stwórz powiadomienie każdego typu, na razie mamy:
    //  - systemowe,
    //  - zaproszenie do znajomych,
    //  - przyjęto zaproszenie do znajomych
    //  - odrzucono zaproszenie do znajomych,
}