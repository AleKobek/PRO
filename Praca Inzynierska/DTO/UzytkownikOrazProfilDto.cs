using Praca_Inzynierska.Models;

namespace Praca_Inzynierska.DTO;

public record UzytkownikOrazProfilDto
(
    int id,
    string login,
    string pseudonim,
    string haslo,
    Region region,
    string? numerTelefonu,
    string? zaimki,
    string? opis,
    ICollection<JezykOrazStopienDto> jezyki
);