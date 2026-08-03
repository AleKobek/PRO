namespace Squadra.Server.Modules.IntegracjeZewnetrzne.DTO;

public record ZewnetrznaStatystykaUzytkownikaDTO(
    int ZewnetrzneIdUzytkownika,
    int StatystykaId,
    string Wartosc,
    double? PorownywalnaWartoscLiczbowa
);