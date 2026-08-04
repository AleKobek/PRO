namespace Squadra.Server.Modules.Statystyki.DTO;

public record StatystykaZNazwaKategoriiDTO(
    int Id,
    string Nazwa,
    int KategoriaId,
    string NazwaKategorii,
    int? RolaId,
    bool CzyToCzasRozgrywki);