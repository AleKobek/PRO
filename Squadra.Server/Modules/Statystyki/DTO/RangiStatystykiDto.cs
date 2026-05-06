namespace Squadra.Server.Modules.Statystyki.DTO;

public record RangiStatystykiDto(
    int IdStatystyki,
    string NazwaStatystyki,
    ICollection<RangaWDtoRangiStatystykiDto> Rangi
);

public record RangaWDtoRangiStatystykiDto(
    string NazwaRangi,
    int WartoscLiczbowa
);