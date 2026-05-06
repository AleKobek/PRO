namespace Squadra.Server.Modules.Statystyki.DTO;

public record RangiStatystykiDto(
    int IdStatystyki,
    ICollection<RangaWDtoRangiStatystykiDto> Rangi
);

public record RangaWDtoRangiStatystykiDto(
    string NazwaRangi,
    int WartoscLiczbowa
);