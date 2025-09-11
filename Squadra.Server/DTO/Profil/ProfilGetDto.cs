namespace Squadra;

public record ProfilGetDto
(
    int IdUzytkownika,
    string Pseudonim,
    RegionDto? Region,
    string? Zaimki,
    string? Opis,
    ICollection<JezykOrazStopienDto> Jezyki,
    byte[]? Awatar
);