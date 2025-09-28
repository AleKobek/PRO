namespace Squadra;

public record ProfilUpdateDto(
    int? RegionId,
    int? KrajId,
    string? Zaimki,
    string? Opis,
    ICollection<JezykOrazStopienDto> Jezyki,
    string Pseudonim,
    byte[]? Awatar
);