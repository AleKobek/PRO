using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Statystyki.DTO;

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
                x.Statystyka.Kategoria.CzyToCzasRozgrywki
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
                x.Statystyka.Kategoria.CzyToCzasRozgrywki
                && x.Statystyka.RolaId == null)
            .Select(x => new CzasRozgrywkiDTO(x.Statystyka.Kategoria.IdGry, int.Parse(x.Wartosc)))
            .ToListAsync();
    }
    
    // get wartość danej statystyki danego użytkownika
    public async Task<string?> GetWartoscStatystyki(int idUzytkownika, int idStatystyki)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik is null)
            throw new NieZnalezionoWBazieException("Użytkownik o id " + idUzytkownika + " nie istnieje.");
        
        var statystyka = await context.Statystyka.FindAsync(idStatystyki);
        if (statystyka is null)
            throw new NieZnalezionoWBazieException("Statystyka o id " + idStatystyki + " nie istnieje.");
        
        return await context.StatystykaUzytkownika
            .Where(x => x.StatystykaId == idStatystyki && x.UzytkownikId == idUzytkownika)
            .Select(x => x.Wartosc)
            .FirstOrDefaultAsync();
    }

    // get statystyki danego użytkownika dla danej gry
    public async Task<ICollection<StatystykaDTO>> GetStatystykiZGry(int idUzytkownika, int idGry)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik is null)
            throw new NieZnalezionoWBazieException("Użytkownik o id " + idUzytkownika + " nie istnieje.");
        
        var gra = await context.WspieranaGra.FindAsync(idGry);
        if (gra is null)
            throw new NieZnalezionoWBazieException("Gra o id " + idGry + " nie istnieje.");
        
        return await context.StatystykaUzytkownika
            .Include(x => x.Statystyka)
            .ThenInclude(s => s.Kategoria)
            // można zrobić include parę razy i cofać się do początku, then include wchodzi głębiej
            .Include(x => x.Statystyka)
            .ThenInclude(s => s.Rola)
            .Where(x =>
                x.UzytkownikId == idUzytkownika &&
                x.Statystyka.Kategoria.IdGry == idGry)
            .Select(x=> new StatystykaDTO(
                x.StatystykaId,
                x.Statystyka.Nazwa,
                x.Wartosc,
                x.Statystyka.Kategoria.Id,
                x.Statystyka.Kategoria.Nazwa,
                x.Statystyka.RolaId,
                x.Statystyka.RolaId == null ? null : x.Statystyka.Rola.Nazwa
            ))
            .ToListAsync();
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
}