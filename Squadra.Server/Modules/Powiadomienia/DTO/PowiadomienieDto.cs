namespace Squadra.Server.DTO.Powiadomienie;

public record PowiadomienieDto(
    int Id,
    int IdTypuPowiadomienia,
    int UzytkownikId,
    int? IdPowiazanegoObiektu,
    string? NazwaPowiazanegoObiektu, // pseudonim dla uzytkownika i nazwa dla gildii
    string? Tresc,
    DateTime DataWyslania
);