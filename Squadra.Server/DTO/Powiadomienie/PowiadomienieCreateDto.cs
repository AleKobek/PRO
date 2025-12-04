namespace Squadra.Server.DTO.Powiadomienie;

public record PowiadomienieCreateDto(
    int IdTypuPowiadomienia, 
    int IdUzytkownika, // dla którego jest powiadomienie
    int? IdPowiazanegoObiektu,
    string? Tresc
);

// 1 - systemowe
// 2 - nowe zaproszenie do znajomych
// 3 - zaakceptowano zaproszenie do znajomych
// 4 - odrzucono zaproszenie do znajomych
// 5 - usunięto cię ze znajomych