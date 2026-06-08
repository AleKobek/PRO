using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Drużyny.DTO;
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
    public async Task<ICollection<StatystykiDoTabelkiDTO>> GetStatystykiUzytkownikaZGry(int idUzytkownika, int idGry)
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
            .Select(x=> new StatystykaUzytkownikaDTO(
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
    
    public async Task<ICollection<Statystyka>> GetStatystykiZGry(int idGry)
    {
        var gra = await context.WspieranaGra.FindAsync(idGry);
        if (gra is null)
            throw new NieZnalezionoWBazieException("Gra o id " + idGry + " nie istnieje.");
        
        return await context.Statystyka
            .Include(s => s.Kategoria)
            .Where(s => s.Kategoria.IdGry == idGry)
            .ToListAsync();
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

    public async Task<bool> CzyUzytkownikSpelniaOgolneWymaganiaDruzyny(int idDruzyny, int idUzytkownika)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik == null) throw new NieZnalezionoWBazieException("Uzytkownik o id " + idUzytkownika + " nie istnieje.");
        
        var druzyna = await context.Druzyna.FindAsync(idDruzyny);
        if (druzyna == null) throw new NieZnalezionoWBazieException("Nie znaleziono drużyny o id " + idDruzyny);

        var wymagania = await GetWymaganiaDruzyny(idDruzyny);
        
        bool czyUzytkownikSpelniaWymagania = true;
        foreach (var w in wymagania){
            var statystykaUzytkownika = await context.StatystykaUzytkownika.FirstOrDefaultAsync(s => s.UzytkownikId == idUzytkownika && s.StatystykaId == w.IdStatystyki);
            if (statystykaUzytkownika == null || statystykaUzytkownika.PorownywalnaWartoscLiczbowa < w.PorownywalnaWartoscLiczbowa)
            {
                czyUzytkownikSpelniaWymagania = false;
                break;
            }
        }
        return czyUzytkownikSpelniaWymagania;
    }
    
    public async Task<bool> CzyUzytkownikSpelniaWymagania(ICollection<WartoscStatystykiDTO> wymagania, int idUzytkownika)
    {
        var statystykiUzytkownika = await context.StatystykaUzytkownika.Where(s => s.UzytkownikId == idUzytkownika).ToListAsync();
        return CzySpelniaWymagania(wymagania, statystykiUzytkownika.Select(s => new WartoscStatystykiDTO(s.StatystykaId, s.Wartosc, s.PorownywalnaWartoscLiczbowa)).ToList());
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
                    ? x.Statystyka.Nazwa
                    : $"{x.Statystyka.Nazwa}({x.Statystyka.Rola.Nazwa})",
                x.Wartosc ?? "brak"
            )).ToListAsync();
        
        return wymaganiaDruzyny;
    }

    public async Task<ICollection<WartoscStatystykiDTO>> GetWymaganiaDruzyny(int idDruzyny)
    {
        var druzyna = await context.Druzyna.FindAsync(idDruzyny);
        if (druzyna == null) throw new NieZnalezionoWBazieException("Nie znaleziono drużyny o id " + idDruzyny);
        
        var wymaganiaDruzyny = await context.WymaganaStatystykaDruzyny
            .Where(m => m.DruzynaId == idDruzyny)
            .Select(x => new WartoscStatystykiDTO(x.StatystykaId, x.Wartosc, x.PorownywalnaWartoscLiczbowa))
            .ToListAsync();

        return wymaganiaDruzyny;
    }
    
    public async Task<string?> GetNazwaRangi(int idStatystyki, int wartoscLiczbowa)
    {
        var statystyka = await context.Statystyka.FindAsync(idStatystyki);
        if (statystyka == null) throw new NieZnalezionoWBazieException("Nie znaleziono statystyki o id " + idStatystyki);
        var ranga = await context.Ranga.Where(x => x.StatystykaId == idStatystyki && x.WartoscLiczbowa <= wartoscLiczbowa).OrderByDescending(x => x.WartoscLiczbowa).FirstOrDefaultAsync();
        // jeżeli ranga jest null, to statystyka nie jest rangą. To posłuży nam do sprawdzenia, czy to ranga
        return ranga?.Nazwa;
    }

    public async Task<ICollection<RangiStatystykiDto>> GetRangiGry(int idGry)
    {
        var rangi = await context.Ranga
            .Include(r => r.Statystyka)
            .ThenInclude(s => s.Kategoria)
            .Include(ranga => ranga.Statystyka).ThenInclude(statystyka => statystyka.Rola)
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
            statystykiDoZwrocenia.Add(new RangiStatystykiDto(
                statystyka.Id,
                statystyka.RolaId == null
                    ? $"{statystyka.Kategoria.Nazwa}: {statystyka.Nazwa}"
                    : $"{statystyka.Kategoria.Nazwa}: {statystyka.Nazwa}({statystyka.Rola.Nazwa})",
                rangiDlaStatystyki));
        }
        return statystykiDoZwrocenia;
    }
    
    // get rangi gry dostępne dla uzytkownika, czyli to samo co wyżej, ale mniejsze lub równe od tych, które ma. potem podmienimy w formularzu
    public async Task<ICollection<RangiStatystykiDto>> GetMniejszeLubRowneRangiGryUzytkownika(int idGry, int idUzytkownika)
    {
        var rangi = await context.Ranga
            .Include(r => r.Statystyka)
            .ThenInclude(s => s.Kategoria)
            .Include(ranga => ranga.Statystyka).ThenInclude(s => s.StatystykaUzytkownikaCollection)
            .Where(r => r.Statystyka.Kategoria.IdGry == idGry && r.Statystyka.StatystykaUzytkownikaCollection.Any(su => su.UzytkownikId == idUzytkownika && su.PorownywalnaWartoscLiczbowa >= r.WartoscLiczbowa))   
            .ToListAsync();
        
        // musimy teraz wziąć statystyki, które są rangami, ale nie ma ich w rangach wyżej, czyli takie, w których wartość liczbową miał null i się nie załapało
        var statystykiZNullami = await context.Statystyka
            .Include(s => s.Kategoria)
            .Include(s => s.RangaCollection)
            .Where(s => s.RangaCollection.Count > 0 && s.Kategoria.IdGry == idGry && s.StatystykaUzytkownikaCollection.Any(su => su.UzytkownikId == idUzytkownika && su.PorownywalnaWartoscLiczbowa == null))
            .ToListAsync();
        
        var statystykiZRangami = rangi.Select(r => r.Statystyka).Distinct().ToList();
        statystykiZRangami.AddRange(statystykiZNullami);
        var statystykiDoZwrocenia = new List<RangiStatystykiDto>();
        foreach (var statystyka in statystykiZRangami)
        {
            var rangiDlaStatystyki = rangi
                .Where(r => r.StatystykaId == statystyka.Id)
                .Select(r => new RangaWDtoRangiStatystykiDto(r.Nazwa, r.WartoscLiczbowa))
                .ToList();
            string? nazwaRoli = null;
            if (statystyka.RolaId != null)
            {
                var rola = await context.Rola.FindAsync(statystyka.RolaId);
                nazwaRoli = rola?.Nazwa;
            }
            statystykiDoZwrocenia.Add(new RangiStatystykiDto(
                statystyka.Id,
                nazwaRoli == null
                    ? $"{statystyka.Kategoria.Nazwa}: {statystyka.Nazwa}"
                    : $"{statystyka.Kategoria.Nazwa}: {statystyka.Nazwa}({nazwaRoli})",
                rangiDlaStatystyki));
        }
        return statystykiDoZwrocenia;
    }
    
    public async Task<ICollection<Rola>> GetRoleGry(int idGry)
    {
        return await context.Rola.Where(r => r.IdGry == idGry).ToListAsync();
    }

    public async Task<ICollection<Rola>> GetRole()
    {
        return  await context.Rola.ToListAsync();
    }
    
    public async Task<Rola> GetRola(int idRoli)
    {
        var rola = await context.Rola.FindAsync(idRoli);
        if (rola == null) throw new NieZnalezionoWBazieException("Nie znaleziono roli o id " + idRoli);
        return rola;
    }

    public ICollection<int> FiltrujNieistniejaceStatystyki(ICollection<int> idStatystyk)
    {
        return idStatystyk.Where(x => !context.Statystyka.Any(s => s.Id == x)).ToList();
    }
    
    public async Task<bool> UsunWymaganeStatystykiDruzyny(int idDruzyny)
    {
        var druzyna = await context.Druzyna.FindAsync(idDruzyny);
        if (druzyna == null) throw new NieZnalezionoWBazieException("Nie znaleziono drużyny o id " + idDruzyny);
        var wymaganeStatystykiDruzyny = context.WymaganaStatystykaDruzyny.Where(w => w.DruzynaId == idDruzyny);
        context.WymaganaStatystykaDruzyny.RemoveRange(wymaganeStatystykiDruzyny);
        await context.SaveChangesAsync();
        return true;
    }
}