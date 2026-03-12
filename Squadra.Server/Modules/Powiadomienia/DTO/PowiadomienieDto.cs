namespace Squadra.Server.Modules.Powiadomienia.DTO;

public record PowiadomienieDto(
    int Id,
    int IdTypuPowiadomienia,
    int UzytkownikId,
    int? IdPowiazanegoObiektu,
    string? NazwaPowiazanegoObiektu, // pseudonim dla uzytkownika i nazwa dla gildii
    string? Tresc,
    string DataWyslania
);