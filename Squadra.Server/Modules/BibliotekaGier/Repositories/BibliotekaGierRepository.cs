using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.BibliotekaGier.DTO;
using Squadra.Server.Modules.BibliotekaGier.Models;
using Squadra.Server.Modules.Platformy.DTO;

namespace Squadra.Server.Modules.BibliotekaGier.Repositories;

public class BibliotekaGierRepository(AppDbContext context) : IBibliotekaGierRepository
{
    public async Task<ICollection<GraWBiblioteceDTO>> PodajGryWBiblioteceUzytkownika(int idUzytkownika)
    {
        
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik is null)
            throw new NieZnalezionoWBazieException("Użytkownik o id " + idUzytkownika + " nie istnieje.");
        
        // tutaj spróbuję z LINQ, nie mam siły zmieniać wszędzie
         var gryUzytkownika = context.GraUzytkownika
            .Where(x => x.UzytkownikId == idUzytkownika)
            .Include(x => x.Gra)
            .Select(
                // dzięki tym include możemy iść po tych wirtualnych obiektach, a nie musimy robić dodatkowych zapytań do bazy
                x => new GraWBiblioteceDTO(
                    x.GraId,
                    x.Gra.Tytul,
                    x.Gra.Gatunek,
                    0, // na razie nie mamy statystyk, więc dajemy 0
                    context.GraUzytkownikaNaPlatformie
                        .Where(gup => gup.UzytkownikId == idUzytkownika && gup.GraId == x.GraId)
                        .Include(gup => gup.Platforma)
                        .Select(gup => new PlatformaWBiblioteceGierDTO(gup.Platforma.Id, gup.Platforma.Nazwa, gup.Platforma.Logo))
                        .ToList()
                )
            ).ToListAsync();
         return await gryUzytkownika;
    }
    
    public async Task<bool> CzyUzytkownikMaDanaGreNaDanejPlatformie(int idUzytkownika, int idGry, int idPlatformy)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik is null)
            throw new NieZnalezionoWBazieException("Użytkownik o id " + idUzytkownika + " nie istnieje.");
        
        var gra = await context.WspieranaGra.FindAsync(idGry);
        if (gra is null)            
            throw new NieZnalezionoWBazieException("Gra o id " + idGry + " nie istnieje.");
        
        var platforma = await context.Platforma.FindAsync(idPlatformy);
        if (platforma is null)            
            throw new NieZnalezionoWBazieException("Platforma o id " + idPlatformy + " nie istnieje.");
        
        var graNaPlatformie = await context.GraUzytkownikaNaPlatformie
            .FirstOrDefaultAsync(gup => gup.UzytkownikId == idUzytkownika && gup.GraId == idGry && gup.PlatformaId == idPlatformy);
        
        return graNaPlatformie != null;
    }
    
    public async Task<bool> CzyUzytkownikMaDanaGre(int idUzytkownika, int idGry)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik is null)
            throw new NieZnalezionoWBazieException("Użytkownik o id " + idUzytkownika + " nie istnieje.");
        
        var gra = await context.WspieranaGra.FindAsync(idGry);
        if (gra is null)            
            throw new NieZnalezionoWBazieException("Gra o id " + idGry + " nie istnieje.");
        
        var graUzytkownika = await context.GraUzytkownika
            .FirstOrDefaultAsync(gu => gu.UzytkownikId == idUzytkownika && gu.GraId == idGry);
        
        return graUzytkownika != null;
    }
    
    public async Task<bool> UpdateBibliotekeGierUzytkownika(int idUzytkownika, List<GraUzytkownikaNaPlatformie> noweGryNaPlatformie, List<GraUzytkownika> noweGry)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik == null) throw new NieZnalezionoWBazieException("Uzytkownik o id " + idUzytkownika + " nie istnieje.");
        
        // usuwamy wszystkie stare wpisy z tabel GraUzytkownikaNaPlatformie i GraUzytkownika dla danego idUzytkownika, aby potem dodać nowe
        
        // najpierw usuwamy gry na platformie
        var stareGryNaPlatformie = await context.GraUzytkownikaNaPlatformie.Where(up => up.UzytkownikId == idUzytkownika).ToListAsync();
        context.GraUzytkownikaNaPlatformie.RemoveRange(stareGryNaPlatformie);
        
        // potem usuwamy gry użytkownika
        var stareGry = await context.GraUzytkownika.Where(up => up.UzytkownikId == idUzytkownika).ToListAsync();
        context.GraUzytkownika.RemoveRange(stareGry);
        
        
        // dodajemy nowe gry użytkownika do bazy danych
        context.GraUzytkownika.AddRange(noweGry);
        await context.SaveChangesAsync();
        
        foreach (var gra in noweGryNaPlatformie)
        {
            gra.UzytkownikId = idUzytkownika;
        }
        
        // potem dodajemy nowe gry na platformie
        context.GraUzytkownikaNaPlatformie.AddRange(noweGryNaPlatformie);
        
        
        await context.SaveChangesAsync();
        
        return true;
    }
    
    // funkcja wyczyść bibliotekę użytkownika, czyli wszystkie jego dane z tabel: GraUzytkownikaNaPlatformie, GraUzytkownika
    public async Task<bool> WyczyscBibliotekeUzytkownika(int idUzytkownika)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik is null)
            throw new NieZnalezionoWBazieException("Użytkownik o id " + idUzytkownika + " nie istnieje.");
        
        var gryUzytkownikaNaPlatformie = context.GraUzytkownikaNaPlatformie.Where(x => x.UzytkownikId == idUzytkownika);
        context.GraUzytkownikaNaPlatformie.RemoveRange(gryUzytkownikaNaPlatformie);
        
        var gryUzytkownika = context.GraUzytkownika.Where(x => x.UzytkownikId == idUzytkownika);
        context.GraUzytkownika.RemoveRange(gryUzytkownika);
        
        await context.SaveChangesAsync();
        
        return true;
    }
}