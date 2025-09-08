namespace Squadra;

public record ProfilUpdateDto(
    int IdUzytkownika,
    int RegionId,
    string? Zaimki,
    string? Opis,
    ICollection<JezykOrazStopienDto> Jezyki,
    byte[]? Awatar
);