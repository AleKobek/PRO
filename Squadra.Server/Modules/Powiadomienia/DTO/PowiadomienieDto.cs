namespace Squadra.Server.Modules.Powiadomienia.DTO;

public record PowiadomienieDto(
    int Id,
    int IdTypuPowiadomienia,
    int UzytkownikId,
    int? IdPowiazanegoObiektu,
    string? NazwaPowiazanegoObiektu, // pseudonim użytkownika
    int? IdDrugiegoPowiazanegoObiektu, // id drugiego użytkownika powiązanego z powiadomieniem (np. w przypadku zaproszenia do drużyny, gdzie powiązany jest zarówno użytkownik, który wysyła zaproszenie, jak i drużyna, do której zaprasza)
    string? NazwaDrugiegoPowiazanegoObiektu, // nazwa drugiego obiektu powiązanego z powiadomieniem (np. nazwa drużyny, do której zaprasza)
    string? Tresc,
    string DataWyslania
);