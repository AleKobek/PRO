
namespace Squadra;

public record UzytkownikOrazProfilDto
(
    int Id,
    string Login,
    string Pseudonim,
    string Haslo,
    RegionDto? Region,
    string? NumerTelefonu,
    string? Zaimki,
    string? Opis,
    ICollection<JezykOrazStopienDto> Jezyki,
    byte[]? Awatar
);