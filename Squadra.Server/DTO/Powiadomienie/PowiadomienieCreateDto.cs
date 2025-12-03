namespace Squadra.Server.DTO.Powiadomienie;

public record PowiadomienieCreateDto(
    int IdTypuPowiadomienia, 
    int IdUzytkownika,
    int? IdPowiazanegoObiektu,
    string? Tresc
);