namespace Squadra.Server.Modules.Powiadomienia.DTO;

public record PowiadomienieCreateDto(
    int IdTypuPowiadomienia, 
    int IdUzytkownika, // dla którego jest powiadomienie
    int? IdPowiazanegoObiektu,
    string? NazwaPowiazanegoObiektu,
    int? IdDrugiegoPowiazanegoObiektu,
    string? NazwaDrugiegoPowiazanegoObiektu,
    string? Tresc // dla powiadomień systemowych lub przenoszenia dodatkowych szczegółów do doklejenia do tekstu
);