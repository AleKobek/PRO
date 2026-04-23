using Microsoft.Data.SqlClient;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.IntegracjeZewnetrzne.DTO;

namespace Squadra.Server.Modules.IntegracjeZewnetrzne.Repositories;

// zastępuje nam zapytania do zewnętrznego serwisu
public class IntegracjeZewnetrzneRepository(IConfiguration configuration) : IIntegracjeZewnetrzneRepository
{
    
    public async Task<DaneZewnetrznegoKontaDTO?> ZwrocDaneKonta(string login)
    {
        try
        {
            await using var con = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
            await con.OpenAsync();
             
            await using var cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT id, haslo_hash FROM zewnetrzne.Konto_Na_Zewnetrznym_Serwisie konto WHERE konto.login = @login;"; 
            cmd.Parameters.AddWithValue("login", login);
            await using var reader = await cmd.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                var idNaZewnetrzymSerwisie = (int)reader["id"];
                var hasloHash = reader["haslo_hash"].ToString() ?? "";
                con.Close();
                return new DaneZewnetrznegoKontaDTO(idNaZewnetrzymSerwisie, hasloHash);
            }
            
            con.Close();

            return null; // nie znaleziono konta o podanych danych logowania
            
        }catch (SqlException e)
        {
            Console.WriteLine($"SQL Error: {e.Message}");
            throw new BladZewnetrznegoSerwisuException("Błąd podczas pobierania danych konta użytkownika z zewnętrznego serwisu.");
        }
    }
    
    public async Task<ICollection<ZewnetrznaPlatformaUzytkownikaDTO>> GetPlatformyUzytkownika(int idNaZewnetrzymSerwisie)
    {
        try
        {
            await using var con = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
            await con.OpenAsync();
             
            await using var cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT id_platformy FROM zewnetrzne.Uzytkownik_Platforma up " +
                              "WHERE up.id_uzytkownika = @idNaZewnetrzymSerwisie AND up.id_platformy IN (SELECT id FROM Platforma)"; 
            cmd.Parameters.AddWithValue("idNaZewnetrzymSerwisie", idNaZewnetrzymSerwisie);
            await using var reader = await cmd.ExecuteReaderAsync();
            
            var platformy = new List<ZewnetrznaPlatformaUzytkownikaDTO>();
            while (await reader.ReadAsync())
            {
                var platformaId = (int)reader["id_platformy"];
                // dodajemy do listy platform użytkownika, które zwrócimy serwisowi
                platformy.Add(new ZewnetrznaPlatformaUzytkownikaDTO(
                        idNaZewnetrzymSerwisie, 
                        platformaId
                ));
            }
            
            con.Close(); // już nam niepotrzebne

            return platformy;
            
        }catch (SqlException e)
        {
            Console.WriteLine($"SQL Error: {e.Message}");
            throw new BladZewnetrznegoSerwisuException("Błąd podczas pobierania platform użytkownika z zewnętrznego serwisu.");
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
            cmd.CommandText = "SELECT id_statystyki, wartosc, porownywalna_wartosc_liczbowa FROM zewnetrzne.Statystyka_Uzytkownika su " +
                              "WHERE su.id_uzytkownika = @idNaZewnetrzymSerwisie AND su.id_statystyki IN (SELECT id FROM Statystyka)"; 
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
            throw new BladZewnetrznegoSerwisuException("Błąd podczas pobierania statystyk użytkownika z zewnętrznego serwisu.");
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
            cmd.CommandText = "SELECT id_gry FROM zewnetrzne.Gra_Uzytkownika su " +
                              "WHERE su.id_uzytkownika = @idNaZewnetrzymSerwisie AND su.id_gry IN (SELECT id FROM Wspierana_gra)"; 
            cmd.Parameters.AddWithValue("idNaZewnetrzymSerwisie", idNaZewnetrzymSerwisie);
            await using var reader = await cmd.ExecuteReaderAsync();
            
            var gry = new List<ZewnetrznaGraUzytkownikaDTO>();
            while (await reader.ReadAsync())
            {
                var idGry = (int)reader["id_gry"];
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
            throw new BladZewnetrznegoSerwisuException("Błąd podczas pobierania biblioteki gier użytkownika z zewnętrznego serwisu.");
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
            cmd.CommandText = "SELECT id_platformy, id_gry FROM zewnetrzne.Gra_Uzytkownika_Na_Platformie su WHERE " +
                              "su.id_uzytkownika = @idNaZewnetrzymSerwisie AND " +
                              "su.id_gry IN (SELECT id FROM Wspierana_gra) AND " +
                              "su.id_platformy IN (SELECT id FROM Platforma)"; 
            cmd.Parameters.AddWithValue("idNaZewnetrzymSerwisie", idNaZewnetrzymSerwisie);
            await using var reader = await cmd.ExecuteReaderAsync();
            
            var gryNaPlatformie = new List<ZewnetrznaGraUzytkownikaNaPlatformieDTO>();
            while (await reader.ReadAsync())
            {
                var idPlatformy = (int)reader["id_platformy"];
                var idGry = (int)reader["id_gry"];
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
            throw new BladZewnetrznegoSerwisuException("Błąd podczas pobierania biblioteki gier użytkownika z zewnętrznego serwisu.");
        }
    }
}