using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Platformy.DTO;
using Squadra.Server.Modules.Platformy.Models;

namespace Squadra.Server.Modules.Platformy.Repositories;

public class PlatformaRepository(AppDbContext context, IConfiguration configuration) : IPlatformaRepository{
    
    public async Task<ICollection<Platforma>> GetPlatformy()
    {
        return await context.Platforma.ToListAsync();
    }
    
    public async Task<Platforma> GetPlatforma(int id)
    {
        var platforma = await context.Platforma.FirstOrDefaultAsync(p => p.Id == id);
        if(platforma is null)
            throw new NieZnalezionoWBazieException("Nie znaleziono platformy o podanym id.");
        return platforma;
    }
    
    public async Task<ICollection<PlatformaUzytkownikaDTO>> GetPlatformyUzytkownika(int idUzytkownika)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik == null) throw new NieZnalezionoWBazieException("Uzytkownik o id " + idUzytkownika + " nie istnieje.");
        
        var platformyUzytkownika = await context.UzytkownikPlatforma.Where(up => up.UzytkownikId == idUzytkownika).ToListAsync();
        var platformy = new List<PlatformaUzytkownikaDTO>();
        foreach (var up in platformyUzytkownika)
        {
            try
            {
                var platforma = await GetPlatforma(up.PlatformaId);
                platformy.Add(new PlatformaUzytkownikaDTO(
                    up.PlatformaId,
                    platforma.Nazwa,
                    platforma.Logo,
                    up.PseudonimNaPlatformie
                ));
            }
            catch (NieZnalezionoWBazieException)
            {
                continue; // jeśli platforma nie istnieje, pomijamy ją
            }
        }
        return platformy;
    }
    
    // funkcja aktualizująca platformy użytkownika, czyli usuwająca wszystkie stare wpisy z tabeli UzytkownikPlatforma dla danego idUzytkownika i dodająca nowe wpisy, które pobieramy z zewnętrznego serwisu
    // potrzebujemy to zrobić ręcznie, gdy użytkownik połączy się po raz pierwszy, aby nie musiał czekać do północy
    public async Task<ICollection<PlatformaUzytkownikaDTO>> UpdatePlatformyUzytkownika(int idUzytkownika)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik == null) throw new NieZnalezionoWBazieException("Uzytkownik o id " + idUzytkownika + " nie istnieje.");
        
        var idNaZewnetrzymSerwisie = uzytkownik.IdNaZewnetrznymSerwisie;
        if (idNaZewnetrzymSerwisie == null) throw new BrakIdNaZewnetrznymSerwisieException("Uzytkownik o id " + idUzytkownika + " nie ma id na zewnętrznym serwisie.");

        try
        {
            await using var con = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
            await con.OpenAsync();
             
            await using var cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT id_platformy, pseudonim_na_platformie FROM zewnetrzne.Uzytkownik_Platforma up WHERE up.id_uzytkownika = @idNaZewnetrzymSerwisie"; 
            cmd.Parameters.AddWithValue("idNaZewnetrzymSerwisie", idNaZewnetrzymSerwisie);
            await using var reader = await cmd.ExecuteReaderAsync();
            
            var platformyDoZwrocenia = new List<PlatformaUzytkownikaDTO>();
            var platformy = new List<UzytkownikPlatforma>();
            while (await reader.ReadAsync())
            {
                var platformaId = (int)reader["id_platformy"];
                var pseudonimNaPlatformie = reader["pseudonim_na_platformie"].ToString() ?? "";
                var platforma = await GetPlatforma(platformaId);
                // dodajemy do listy do zwrócenia, czyli do listy platform użytkownika, które zwrócimy do frontendu
                platformyDoZwrocenia.Add(new PlatformaUzytkownikaDTO(
                    platformaId,
                    platforma.Nazwa,
                    platforma.Logo,
                    pseudonimNaPlatformie 
                ));
                // dodajemy do listy platform użytkownika, które dodamy do bazy danych, czyli do tabeli UzytkownikPlatforma
                platformy.Add(new UzytkownikPlatforma
                {
                    UzytkownikId = idUzytkownika,
                    PlatformaId = platformaId,
                    PseudonimNaPlatformie = pseudonimNaPlatformie
                });
            }
            
            await using var transaction = await context.Database.BeginTransactionAsync();
            
            // usuwamy wszystkie stare wpisy z tabeli UzytkownikPlatforma dla danego idUzytkownika,
            // czyli usuwamy wszystkie platformy użytkownika, które mamy w bazie danych, żeby potem dodać nowe, które pobraliśmy z zewnętrznego serwisu
            var starePlatformyUzytkownika = await context.UzytkownikPlatforma.Where(up => up.UzytkownikId == idUzytkownika).ToListAsync();
            context.UzytkownikPlatforma.RemoveRange(starePlatformyUzytkownika);
            
            // dodajemy wszystkie platformy użytkownika do bazy danych, czyli do tabeli UzytkownikPlatforma
            context.UzytkownikPlatforma.AddRange(platformy);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return platformyDoZwrocenia;
            
        }catch (SqlException e)
        {
            Console.WriteLine($"SQL Error: {e.Message}");
            throw new Exception("Błąd podczas pobierania platform użytkownika z zewnętrznego serwisu.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            throw new Exception("Nieoczekiwany błąd podczas pobierania platform użytkownika z zewnętrznego serwisu.");
        }
    }
    
    // funkcja usuwająca wszystkie platformy użytkownika, czyli wszystkie wpisy z tabeli UzytkownikPlatforma dla danego idUzytkownika
    // zakładamy, że usunęliśmy już wszystkie gry na platformach użytkownika, więc nie musimy się martwić o blokady
    public async Task<bool> UsunPlatformyUzytkownika(int idUzytkownika)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik == null) throw new NieZnalezionoWBazieException("Uzytkownik o id " + idUzytkownika + " nie istnieje.");
        
        var platformyUzytkownika = await context.UzytkownikPlatforma.Where(up => up.UzytkownikId == idUzytkownika).ToListAsync();
        context.UzytkownikPlatforma.RemoveRange(platformyUzytkownika);
        await context.SaveChangesAsync();
        return true;
    }
}