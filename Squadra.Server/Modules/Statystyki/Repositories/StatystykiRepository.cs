using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Statystyki.DTO;
using Squadra.Server.Modules.Statystyki.Models;

namespace Squadra.Server.Modules.Statystyki.Repositories;

public class StatystykiRepository(AppDbContext context) : IStatystykiRepository
{
    
    // get godziny grania danego użytkownika dla danej gry
    public async Task<string> GetGodzinyGrania(int idUzytkownika, int idGry)
    {
        
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik is null)
            throw new NieZnalezionoWBazieException("Użytkownik o id " + idUzytkownika + " nie istnieje.");
        
        var gra = await context.WspieranaGra.FindAsync(idGry);
        if (gra is null)
            throw new NieZnalezionoWBazieException("Gra o id " + idGry + " nie istnieje.");
        
        var godzinyGrania = await context.StatystykaUzytkownika
            .Include(x => x.Statystyka)
            .ThenInclude(x => x.Kategoria)
            .Where(x =>
                x.UzytkownikId == idUzytkownika &&
                x.Statystyka.Kategoria.IdGry == idGry &&
                x.Statystyka.CzyToCzasRozgrywki
                && x.Statystyka.RolaId == null)
            .Select(x => x.Wartosc)
            .FirstOrDefaultAsync();
        
        return string.IsNullOrEmpty(godzinyGrania) ? "0" : godzinyGrania;
    }
    
    //get wszystkie czasy rozgrywek gier danego użytkownika
    public async Task<ICollection<CzasRozgrywkiDTO>> GetGodzinyGraniaUzytkownika(int idUzytkownika)
    {
        
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik is null)
            throw new NieZnalezionoWBazieException("Użytkownik o id " + idUzytkownika + " nie istnieje.");
        
        return await context.StatystykaUzytkownika
            .Include(x => x.Statystyka)
            .ThenInclude(x => x.Kategoria)
            .Where(x =>
                x.UzytkownikId == idUzytkownika &&
                x.Statystyka.CzyToCzasRozgrywki
                && x.Statystyka.RolaId == null)
            .Select(x => new CzasRozgrywkiDTO(x.Statystyka.Kategoria.IdGry, (int)Math.Floor(x.PorownywalnaWartoscLiczbowa ?? 0)))
            .ToListAsync();
    }
    
    // get wartość danej statystyki danego użytkownika
    public async Task<WartoscStatystykiDTO?> GetWartoscStatystyki(int idUzytkownika, int idStatystyki)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik is null)
            throw new NieZnalezionoWBazieException("Użytkownik o id " + idUzytkownika + " nie istnieje.");
        
        var statystyka = await context.Statystyka.FindAsync(idStatystyki);
        if (statystyka is null)
            throw new NieZnalezionoWBazieException("Statystyka o id " + idStatystyki + " nie istnieje.");
        
        return await context.StatystykaUzytkownika
            .Where(x => x.StatystykaId == idStatystyki && x.UzytkownikId == idUzytkownika)
            .Select(x => new WartoscStatystykiDTO(x.StatystykaId, x.Wartosc, x.PorownywalnaWartoscLiczbowa))
            .FirstOrDefaultAsync();
    }

    // get statystyki danego użytkownika dla danej gry
    public async Task<ICollection<StatystykiDoTabelkiDTO>> GetStatystykiZGry(int idUzytkownika, int idGry)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik is null)
            throw new NieZnalezionoWBazieException("Użytkownik o id " + idUzytkownika + " nie istnieje.");
        
        var gra = await context.WspieranaGra.FindAsync(idGry);
        if (gra is null)
            throw new NieZnalezionoWBazieException("Gra o id " + idGry + " nie istnieje.");
        
        var statystyki = await context.StatystykaUzytkownika
            .Include(x => x.Statystyka)
            .ThenInclude(s => s.Kategoria)
            // można zrobić include parę razy i cofać się do początku, then include wchodzi głębiej
            .Include(x => x.Statystyka)
            .ThenInclude(s => s.Rola)
            .Where(x =>
                x.UzytkownikId == idUzytkownika &&
                x.Statystyka.Kategoria.IdGry == idGry)
            .OrderBy(x => x.StatystykaId)
            .Select(x=> new StatystykaDTO(
                x.StatystykaId,
                x.Statystyka.Nazwa,
                x.Wartosc,
                x.PorownywalnaWartoscLiczbowa,
                x.Statystyka.Kategoria.Id,
                x.Statystyka.Kategoria.Nazwa,
                x.Statystyka.RolaId,
                x.Statystyka.RolaId == null ? null : x.Statystyka.Rola.Nazwa
            ))
            .ToListAsync();
        var kategorie = statystyki.Select(s => new { s.KategoriaId, s.KategoriaNazwa }).Distinct().ToList();
        return kategorie.Select(k => new StatystykiDoTabelkiDTO(
            k.KategoriaId,
            k.KategoriaNazwa,
            statystyki.Where(s => s.KategoriaId == k.KategoriaId).ToList()
        )).ToList();
    }
    
    // funkcja aktualizująca statystyki użytkownika, czyli usuwająca wszystkie stare wpisy z tabeli StatystykaUzytkownika dla danego idUzytkownika i dodająca nowe wpisy, które pobieramy z zewnętrznego serwisu
    // potrzebujemy to zrobić ręcznie, gdy użytkownik połączy się po raz pierwszy, aby nie musiał czekać do północy
    public async Task<bool> UpdateStatystykiUzytkownika(int idUzytkownika, List<StatystykaUzytkownika> noweStatystyki)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik == null) throw new NieZnalezionoWBazieException("Uzytkownik o id " + idUzytkownika + " nie istnieje.");
        
        
        // usuwamy wszystkie stare wpisy z tabeli StatystykaUzytkownika dla danego idUzytkownika
        var stareStatystykiUzytkownika = await context.StatystykaUzytkownika.Where(up => up.UzytkownikId == idUzytkownika).ToListAsync();
        context.StatystykaUzytkownika.RemoveRange(stareStatystykiUzytkownika);
        
        // dodajemy wszystkie nowe statystyki użytkownika do bazy danych
        context.StatystykaUzytkownika.AddRange(noweStatystyki);
        await context.SaveChangesAsync();
        
        return true;
            
    }
    
    public async Task<bool> UsunStatystykiUzytkownika(int idUzytkownika)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik is null)
            throw new NieZnalezionoWBazieException("Użytkownik o id " + idUzytkownika + " nie istnieje.");
        
        var statystykiUzytkownika = context.StatystykaUzytkownika.Where(x => x.UzytkownikId == idUzytkownika);
        context.StatystykaUzytkownika.RemoveRange(statystykiUzytkownika);
        await context.SaveChangesAsync();
        return true;
    }
    
    public bool CzySpelniaWymagania(ICollection<WartoscStatystykiDTO> wymagania, ICollection<WartoscStatystykiDTO> statystykiDoSprawdzenia) {
        foreach (var w in wymagania)
        {
            var statystyka = statystykiDoSprawdzenia.FirstOrDefault(s => s.IdStatystyki == w.IdStatystyki);
            if (statystyka == null || statystyka.PorownywalnaWartoscLiczbowa < w.PorownywalnaWartoscLiczbowa)
            {
                return false;
            }
        }
        return true;
    }
}