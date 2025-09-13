namespace Squadra;

public record ProfilGetResDto
(
    int IdUzytkownika,
    string Pseudonim,
    RegionKrajDto? Region,
    string? Zaimki,
    string? Opis,
    ICollection<JezykOrazStopienDto> Jezyki,
    byte[]? Awatar
);