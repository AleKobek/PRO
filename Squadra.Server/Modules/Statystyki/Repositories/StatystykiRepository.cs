using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Statystyki.DTO;
using Squadra.Server.Modules.Statystyki.Models;

namespace Squadra.Server.Modules.Statystyki.Repositories;

public class StatystykiRepository(AppDbContext context, IConfiguration configuration) : IStatystykiRepository
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
                x.PorownywalnaWartoscLiczbowa,
                x.Statystyka.Kategoria.Id,
                x.Statystyka.Kategoria.Nazwa,
                x.Statystyka.RolaId,
                x.Statystyka.RolaId == null ? null : x.Statystyka.Rola.Nazwa
            ))
            .ToListAsync();
    }
    
    // funkcja aktualizująca statystyki użytkownika, czyli usuwająca wszystkie stare wpisy z tabeli StatystykaUzytkownika dla danego idUzytkownika i dodająca nowe wpisy, które pobieramy z zewnętrznego serwisu
    // potrzebujemy to zrobić ręcznie, gdy użytkownik połączy się po raz pierwszy, aby nie musiał czekać do północy
    public async Task<ICollection<StatystykaUzytkownika>> UpdateStatystykiUzytkownika(int idUzytkownika)
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
            cmd.CommandText = "SELECT id_statystyki, wartosc, porownywalna_wartosc_liczbowa FROM zewnetrzne.Statystyka_Uzytkownika su WHERE su.id_uzytkownika = @idNaZewnetrzymSerwisie"; 
            cmd.Parameters.AddWithValue("idNaZewnetrzymSerwisie", idNaZewnetrzymSerwisie);
            await using var reader = await cmd.ExecuteReaderAsync();
            
            var statystyki = new List<StatystykaUzytkownika>();
            while (await reader.ReadAsync())
            {
                var statystykaId = (int)reader["id_statystyki"];
                var wartoscStatystyki = reader["wartosc"].ToString() ?? "";
                var porownywalnaWartoscLiczbowa = reader["porownywalna_wartosc_liczbowa"] as int?;
                // dodajemy do listy statystyk użytkownika, które dodamy do bazy danych, czyli do tabeli StatystykaUzytkownika
                statystyki.Add(new StatystykaUzytkownika
                {
                    UzytkownikId = idUzytkownika,
                    StatystykaId =  statystykaId,
                    Wartosc = wartoscStatystyki,
                    PorownywalnaWartoscLiczbowa = porownywalnaWartoscLiczbowa
                });
            }
            
            con.Close(); // już nam niepotrzebne
            
            await using var transaction = await context.Database.BeginTransactionAsync();
            
            // usuwamy wszystkie stare wpisy z tabeli StatystykaUzytkownika dla danego idUzytkownika,
            // czyli usuwamy statystyki platformy użytkownika, które mamy w bazie danych, żeby potem dodać nowe, które pobraliśmy z zewnętrznego serwisu
            var stareStatystykiUzytkownika = await context.StatystykaUzytkownika.Where(up => up.UzytkownikId == idUzytkownika).ToListAsync();
            context.StatystykaUzytkownika.RemoveRange(stareStatystykiUzytkownika);
            
            // dodajemy wszystkie statystyki użytkownika do bazy danych, czyli do tabeli UzytkownikPlatforma
            context.StatystykaUzytkownika.AddRange(statystyki);
            await context.SaveChangesAsync();
            
            await transaction.CommitAsync();
            
            return statystyki;
            
        }catch (SqlException e)
        {
            Console.WriteLine($"SQL Error: {e.Message}");
            throw new Exception("Błąd podczas pobierania statystyk użytkownika z zewnętrznego serwisu.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            throw new Exception("Nieoczekiwany błąd podczas pobierania statystyk użytkownika z zewnętrznego serwisu.");
        }
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