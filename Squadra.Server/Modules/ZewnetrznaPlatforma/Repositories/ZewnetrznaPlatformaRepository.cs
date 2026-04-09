using Microsoft.Data.SqlClient;
using Squadra.Server.Modules.ZewnetrznaPlatforma.DTO;

namespace Squadra.Server.Modules.ZewnetrznaPlatforma.Repositories;

// zastępuje nam zapytania do zewnętrznego serwisu
public class ZewnetrznaPlatformaRepository(IConfiguration configuration) : IZewnetrznaPlatformaRepository
{
    public async Task<ICollection<ZewnetrznaPlatformaUzytkownikaDTO>> GetPlatformyUzytkownika(int idNaZewnetrzymSerwisie)
    {
        try
        {
            await using var con = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
            await con.OpenAsync();
             
            await using var cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT id_platformy, pseudonim_na_platformie FROM zewnetrzne.Uzytkownik_Platforma up WHERE up.id_uzytkownika = @idNaZewnetrzymSerwisie"; 
            cmd.Parameters.AddWithValue("idNaZewnetrzymSerwisie", idNaZewnetrzymSerwisie);
            await using var reader = await cmd.ExecuteReaderAsync();
            
            var platformy = new List<ZewnetrznaPlatformaUzytkownikaDTO>();
            while (await reader.ReadAsync())
            {
                var platformaId = (int)reader["id_platformy"];
                var pseudonimNaPlatformie = reader["pseudonim_na_platformie"].ToString() ?? "";
                // dodajemy do listy platform użytkownika, które zwrócimy serwisowi
                platformy.Add(new ZewnetrznaPlatformaUzytkownikaDTO(
                        idNaZewnetrzymSerwisie, 
                        platformaId, 
                        pseudonimNaPlatformie
                ));
            }
            
            con.Close(); // już nam niepotrzebne

            return platformy;
            
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
    
    public async Task<ICollection<ZewnetrznaStatystykaUzytkownikaDTO>> GetStatystykiUzytkownika(int idNaZewnetrzymSerwisie)
    {
        try
        {
            await using var con = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
            await con.OpenAsync();
             
            await using var cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT id_statystyki, wartosc, porownywalna_wartosc_liczbowa FROM zewnetrzne.Statystyka_Uzytkownika su WHERE su.id_uzytkownika = @idNaZewnetrzymSerwisie"; 
            cmd.Parameters.AddWithValue("idNaZewnetrzymSerwisie", idNaZewnetrzymSerwisie);
            await using var reader = await cmd.ExecuteReaderAsync();
            
            var statystyki = new List<ZewnetrznaStatystykaUzytkownikaDTO>();
            while (await reader.ReadAsync())
            {
                var statystykaId = (int)reader["id_statystyki"];
                var wartoscStatystyki = reader["wartosc"].ToString() ?? "";
                var porownywalnaWartoscLiczbowa = reader["porownywalna_wartosc_liczbowa"] as int?;
                // dodajemy do listy statystyk użytkownika, które dodamy do bazy danych, czyli do tabeli StatystykaUzytkownika
                statystyki.Add(new ZewnetrznaStatystykaUzytkownikaDTO(
                    idNaZewnetrzymSerwisie,
                    statystykaId,
                    wartoscStatystyki,
                    porownywalnaWartoscLiczbowa
                ));

            }
            
            con.Close(); // już nam niepotrzebne
            
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

    public async Task<ICollection<ZewnetrznaGraUzytkownikaDTO>> GetGryUzytkownika(int idNaZewnetrzymSerwisie)
    {
        try
        {
            await using var con = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
            await con.OpenAsync();
            
            // pobierany gry w bibliotece
            await using var cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT id_wspieranej_gry FROM zewnetrzne.Gra_Uzytkownika su WHERE su.id_uzytkownika = @idNaZewnetrzymSerwisie"; 
            cmd.Parameters.AddWithValue("idNaZewnetrzymSerwisie", idNaZewnetrzymSerwisie);
            await using var reader = await cmd.ExecuteReaderAsync();
            
            var gry = new List<ZewnetrznaGraUzytkownikaDTO>();
            while (await reader.ReadAsync())
            {
                var idGry = (int)reader["id_wspieranej_gry"];
                gry.Add(new ZewnetrznaGraUzytkownikaDTO
                (
                    idNaZewnetrzymSerwisie,
                    idGry
                ));
            }
            
            con.Close(); // już nam niepotrzebne
            
            return gry;
            
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

    public async Task<ICollection<ZewnetrznaGraUzytkownikaNaPlatformieDTO>> GetGryUzytkownikaNaPlatformie(int idNaZewnetrzymSerwisie)
    {
        try
        {
            await using var con = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
            await con.OpenAsync();
            
            await using var cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT id_platformy, id_wspieranej_gry FROM zewnetrzne.Gra_Uzytkownika_Na_Platformie su WHERE su.id_uzytkownika = @idNaZewnetrzymSerwisie"; 
            cmd.Parameters.AddWithValue("idNaZewnetrzymSerwisie", idNaZewnetrzymSerwisie);
            await using var reader = await cmd.ExecuteReaderAsync();
            
            var gryNaPlatformie = new List<ZewnetrznaGraUzytkownikaNaPlatformieDTO>();
            while (await reader.ReadAsync())
            {
                var idPlatformy = (int)reader["id_platformy"];
                var idGry = (int)reader["id_wspieranej_gry"];
                // dodajemy do listy gier na platformie użytkownika, które dodamy do bazy danych, czyli do tabeli GraUzytkownikaNaPlatformie
                gryNaPlatformie.Add(new ZewnetrznaGraUzytkownikaNaPlatformieDTO
                (
                    idNaZewnetrzymSerwisie,
                    idGry,
                    idPlatformy
                ));
            }
            
            con.Close(); // już nam niepotrzebne
            
            return gryNaPlatformie;
            
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
}