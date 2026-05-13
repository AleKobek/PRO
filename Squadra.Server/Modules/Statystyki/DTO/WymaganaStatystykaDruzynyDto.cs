namespace Squadra.Server.Modules.Statystyki.DTO;

public record WymaganaStatystykaDruzynyDto(
    int? DruzynaId, // przy tworzeniu drużyny ona jeszcze nie ma id
    int StatystykaId,
    string? Wartosc,
    double? PorownywalnaWartoscLiczbowa
);