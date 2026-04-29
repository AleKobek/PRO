namespace Squadra.Server.Modules.Statystyki.DTO;

public record WartoscStatystykiDTO(
    int IdStatystyki,
    string? Wartosc,
    double? PorownywalnaWartoscLiczbowa
);