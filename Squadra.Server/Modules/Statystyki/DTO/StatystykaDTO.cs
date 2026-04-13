namespace Squadra.Server.Modules.Statystyki.DTO;

public record StatystykaDTO(
    int Id,
    string Nazwa,
    string Wartosc,
    double? PorownywalnaWartoscLiczbowa,
    int KategoriaId,
    string KategoriaNazwa,
    int? RolaId,
    string? RolaNazwa
);