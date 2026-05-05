namespace Squadra.Server.Modules.Statystyki.DTO;

public record StatystykiDoFormularzaDto(
    ICollection<StatystykaDoFormularzaNieBedacaRangaDto> StatystykiNieBedaceRangami,
    ICollection<RangiStatystykiDto> Rangi
);

public record StatystykaDoFormularzaNieBedacaRangaDto(
    int Id,
    string Nazwa
);