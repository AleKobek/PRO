namespace Squadra.Server.Modules.Statystyki.DTO;

public record StatystykiDoFormularzaDto(
    ICollection<StatystykaDoFormularzaNieBedacaRangaDto> StatystykiNieBedaceRangami,
    ICollection<RangiStatystykiDto> Rangi,
    ICollection<RangiStatystykiDto> WszystkieRangi);

public record StatystykaDoFormularzaNieBedacaRangaDto(
    int Id,
    string Nazwa
);