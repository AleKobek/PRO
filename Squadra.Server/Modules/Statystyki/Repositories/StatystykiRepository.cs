using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Statystyki.DTO;
using Squadra.Server.Modules.Statystyki.Models;

namespace Squadra.Server.Modules.Statystyki.Repositories;

public class StatystykiRepository(AppDbContext context) : IStatystykiRepository
{
    
    public async Task<Statystyka> GetStatystyka(int idStatystyki)
    {
        var statystyka = await context.Statystyka.FindAsync(idStatystyki);
        if (statystyka == null) throw new NieZnalezionoWBazieException("Nie znaleziono statystyki o id " + idStatystyki);
        return statystyka;
    }
    
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
                x.Statystyka.RolaId == null 
                    ? x.StatystykaId == 11 ? x.Statystyka.Nazwa + " (otwarta rywalizacja)" : x.Statystyka.Nazwa 
                    : x.Statystyka.Nazwa + " (" + x.Statystyka.Rola.Nazwa + ")",
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
    
    public async Task<ICollection<WymaganieDruzynyDoWyswietleniaDto>> GetWymaganiaDruzynyDoWyswietlenia(int idDruzyny)
    {
        var druzyna = await context.Druzyna.FindAsync(idDruzyny);
        if (druzyna == null) throw new NieZnalezionoWBazieException("Nie znaleziono drużyny o id " + idDruzyny);

        var wymaganiaDruzyny = await context.WymaganaStatystykaDruzyny
            .Where(m => m.DruzynaId == idDruzyny)
            .Include(x => x.Statystyka)
            .ThenInclude(s => s.Rola)
            .Select(x => new WymaganieDruzynyDoWyswietleniaDto(
                x.StatystykaId,
                x.Statystyka.RolaId == null
                    ? null
                    : $"{x.Statystyka.Nazwa}({x.Statystyka.Rola.Nazwa})",
                x.Wartosc ?? "brak"
            )).ToListAsync();
        
        return wymaganiaDruzyny;
    }
    
    public async Task<ICollection<Ranga>> GetRangiStatystyki(int idStatystyki)
    {
        var ranga = await context.Ranga.Where(x => x.StatystykaId == idStatystyki).ToListAsync();
        if (ranga.Count == 0) throw new NieZnalezionoWBazieException("Nie znaleziono rang statystyki o id " + idStatystyki);
        
        return ranga;
    }
    
    public async Task<string?> GetNazwaRangi(int idStatystyki, int wartoscLiczbowa)
    {
        var statystyka = await context.Statystyka.FindAsync(idStatystyki);
        if (statystyka == null) throw new NieZnalezionoWBazieException("Nie znaleziono statystyki o id " + idStatystyki);
        var ranga = await context.Ranga.Where(x => x.StatystykaId == idStatystyki && x.WartoscLiczbowa <= wartoscLiczbowa).OrderByDescending(x => x.WartoscLiczbowa).FirstOrDefaultAsync();
        // jeżeli ranga jest null, to statystyka nie jest rangą. To posłuży nam do sprawdzenia, czy to ranga
        return ranga?.Nazwa;
    }
    
    public async Task<RangiStatystykiDto> GetRangiStatystykiDto(int idStatystyki)
    {
        var rangi = await context.Ranga.Where(x => x.StatystykaId == idStatystyki).ToListAsync();
        if (rangi.Count == 0) throw new NieZnalezionoWBazieException("Nie znaleziono rang statystyki o id " + idStatystyki);
        
        return new RangiStatystykiDto(idStatystyki, rangi.Select(r => new RangaWDtoRangiStatystykiDto(r.Nazwa, r.WartoscLiczbowa)).ToList());
    }

    public async Task<ICollection<RangiStatystykiDto>> GetRangiGry(int idGry)
    {
        var rangi = await context.Ranga
            .Include(r => r.Statystyka)
            .ThenInclude(s => s.Kategoria)
            .Where(r => r.Statystyka.Kategoria.IdGry == idGry)
            .ToListAsync();
        
        var statystykiZRangami = rangi.Select(r => r.Statystyka).Distinct().ToList();
        var statystykiDoZwrocenia = new List<RangiStatystykiDto>();
        foreach (var statystyka in statystykiZRangami)
        {
            var rangiDlaStatystyki = rangi
                .Where(r => r.StatystykaId == statystyka.Id)
                .Select(r => new RangaWDtoRangiStatystykiDto(r.Nazwa, r.WartoscLiczbowa))
                .ToList();
            statystykiDoZwrocenia.Add(new RangiStatystykiDto(statystyka.Id, rangiDlaStatystyki));
        }
        return statystykiDoZwrocenia;
    }
    
    // funkcja do zwracania statystyk do formularza. musimy oddzielić rangi od reszty statystyk, bo je się wyświetla inaczej
    public async Task<StatystykiDoFormularzaDto> GetStatystykiDoFormularza(int idUzytkownika, int idGry)
    {
        var statystyki = await GetStatystykiZGry(idUzytkownika, idGry);
        var rangi = await GetRangiGry(idGry);
        var statystykiBezRang = statystyki
            .SelectMany(s => s.Statystyki) // spłaszczamy listę statystyk, nie dzielimy ich na kategori
            // All zwraca true, jeśli wszystkie elementy spełniają warunek.
            // tutaj sprawdzamy, czy statystyka nie jest rangą, czyli czy nie ma rangi o takim samym id statystyki
            .Where(s => rangi.All(r => r.IdStatystyki != s.Id)) 
            .Select(s => new StatystykaDoFormularzaNieBedacaRangaDto(
                s.Id,
                s.Nazwa
            ))
            .ToList();
        return new StatystykiDoFormularzaDto(statystykiBezRang, rangi);
    }
}