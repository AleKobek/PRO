namespace Squadra.Server.Modules.Statystyki.DTO;

public record StatystykaDTO(
    int Id,
    string Nazwa,
    int KategoriaId,
    int? RolaId,
    bool CzyToCzasRozgrywki
);