namespace Squadra.Server.Modules.Statystyki.DTO;

public record RangiStatystykiDto(
    int Id,
    string Nazwa,
    ICollection<RangaWDtoRangiStatystykiDto> Rangi
);

public record RangaWDtoRangiStatystykiDto(
    string NazwaRangi,
    int WartoscLiczbowa
);