using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.BibliotekaGier.DTO;
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
            .ThenInclude(g => g.GraNaPlatformieCollection)
            .ThenInclude(gp => gp.Platforma)
            .Select(
                // dzięki tym include możemy iść po tych wirtualnych obiektach, a nie musimy robić dodatkowych zapytań do bazy
                x => new GraWBiblioteceDTO(
                    x.GraId,
                    x.Gra.Tytul,
                    x.Gra.Gatunek,
                    0, // na razie nie mamy statystyk, więc dajemy 0
                    x.Gra.GraNaPlatformieCollection
                        .Select(gp => 
                            new PlatformaWBiblioteceGierDTO(gp.Platforma.Id, gp.Platforma.Nazwa, gp.Platforma.Logo))
                        .ToList()
                )
            ).ToListAsync();
         return await gryUzytkownika;
    }
}