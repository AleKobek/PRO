using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.BibliotekaGier.DTO;
using Squadra.Server.Modules.BibliotekaGier.Models;
using Squadra.Server.Modules.Platformy.DTO;

namespace Squadra.Server.Modules.BibliotekaGier.Repositories;

public class BibliotekaGierRepository(AppDbContext context, IConfiguration configuration) : IBibliotekaGierRepository
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
    
    public async Task<bool> UpdateBibliotekeGierUzytkownika(int idUzytkownika)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik == null) throw new NieZnalezionoWBazieException("Uzytkownik o id " + idUzytkownika + " nie istnieje.");
        
        var idNaZewnetrzymSerwisie = uzytkownik.IdNaZewnetrznymSerwisie;
        if (idNaZewnetrzymSerwisie == null) throw new BrakIdNaZewnetrznymSerwisieException("Uzytkownik o id " + idUzytkownika + " nie ma id na zewnętrznym serwisie.");

        try
        {
            await using var con = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
            await con.OpenAsync();
            
            // pobieramy gry na platformie
            await using var cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT id_platformy, id_wspieranej_gry FROM zewnetrzne.Gra_Uzytkownika_Na_Platformie su WHERE su.id_uzytkownika = @idNaZewnetrzymSerwisie"; 
            cmd.Parameters.AddWithValue("idNaZewnetrzymSerwisie", idNaZewnetrzymSerwisie);
            await using var reader = await cmd.ExecuteReaderAsync();
            
            var gryNaPlatformie = new List<GraUzytkownikaNaPlatformie>();
            while (await reader.ReadAsync())
            {
                var idPlatformy = (int)reader["id_platformy"];
                var idGry = (int)reader["id_wspieranej_gry"];
                // dodajemy do listy gier na platformie użytkownika, które dodamy do bazy danych, czyli do tabeli GraUzytkownikaNaPlatformie
                gryNaPlatformie.Add(new GraUzytkownikaNaPlatformie
                {
                    UzytkownikId = idUzytkownika,
                    PlatformaId =  idPlatformy,
                    GraId = idGry
                });
            }
            
            // pobierany gry w bibliotece
            await using var cmd2 = new SqlCommand();
            cmd2.Connection = con;
            cmd2.CommandText = "SELECT id_wspieranej_gry FROM zewnetrzne.Gra_Uzytkownika su WHERE su.id_uzytkownika = @idNaZewnetrzymSerwisie"; 
            cmd2.Parameters.AddWithValue("idNaZewnetrzymSerwisie", idNaZewnetrzymSerwisie);
            await using var reader2 = await cmd2.ExecuteReaderAsync();
            
            var gry = new List<GraUzytkownika>();
            while (await reader2.ReadAsync())
            {
                var idGry = (int)reader["id_wspieranej_gry"];
                // dodajemy do listy gier użytkownika, które dodamy do bazy danych, czyli do tabeli GraUzytkownika
                gry.Add(new GraUzytkownika
                {
                    UzytkownikId = idUzytkownika,
                    GraId = idGry
                });
            }
            
            con.Close(); // już nam niepotrzebne
            
            await using var transaction = await context.Database.BeginTransactionAsync();
            
            // usuwamy wszystkie stare wpisy z tabel GraUzytkownikaNaPlatformie i GraUzytkownika dla danego idUzytkownika, aby potem dodać nowe
            
            // najpierw usuwamy gry na platformie
            var stareGryNaPlatformie = await context.GraUzytkownikaNaPlatformie.Where(up => up.UzytkownikId == idUzytkownika).ToListAsync();
            context.GraUzytkownikaNaPlatformie.RemoveRange(stareGryNaPlatformie);
            
            // potem usuwamy gry użytkownika
            var stareGry = await context.GraUzytkownika.Where(up => up.UzytkownikId == idUzytkownika).ToListAsync();
            context.GraUzytkownika.RemoveRange(stareGry);
            
            
            // dodajemy nowe gry użytkownika do bazy danych
            context.GraUzytkownika.AddRange(gry);
            
            // potem dodajemy nowe gry na platformie
            context.GraUzytkownikaNaPlatformie.AddRange(gryNaPlatformie);
            
            
            await context.SaveChangesAsync();
            
            await transaction.CommitAsync();
            
            return true;
            
        }catch (SqlException e)
        {
            Console.WriteLine($"SQL Error: {e.Message}");
            throw new Exception("Błąd podczas pobierania biblioteki gier użytkownika z zewnętrznego serwisu.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            throw new Exception("Nieoczekiwany błąd podczas biblioteki gier użytkownika z zewnętrznego serwisu.");
        }
    }
    
    // funkcja wyczyść bibliotekę użytkownika, czyli wszystkie jego dane z tabel: GraUzytkownikaNaPlatformie, GraUzytkownika
    public async Task<bool> WyczyscBibliotekeUzytkownika(int idUzytkownika)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik is null)
            throw new NieZnalezionoWBazieException("Użytkownik o id " + idUzytkownika + " nie istnieje.");
        
        await using var transaction = await context.Database.BeginTransactionAsync();
        var gryUzytkownikaNaPlatformie = context.GraUzytkownikaNaPlatformie.Where(x => x.UzytkownikId == idUzytkownika);
        context.GraUzytkownikaNaPlatformie.RemoveRange(gryUzytkownikaNaPlatformie);
        
        var gryUzytkownika = context.GraUzytkownika.Where(x => x.UzytkownikId == idUzytkownika);
        context.GraUzytkownika.RemoveRange(gryUzytkownika);
        
        await transaction.CommitAsync();
        await context.SaveChangesAsync();
        
        return true;
    }
}