namespace Squadra.Server.DTO.Powiadomienie;

public record PowiadomienieDto(
    int Id,
    string NazwaTypu,
    int UzytkownikId,
    int? IdPowiazanegoObiektu,
    string? NazwaPowiazanegoObiektu,
    string Tresc,
    DateTime DataWyslania
);