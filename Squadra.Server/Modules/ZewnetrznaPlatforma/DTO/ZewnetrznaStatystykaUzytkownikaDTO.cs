namespace Squadra.Server.Modules.ZewnetrznaPlatforma.DTO;

public record ZewnetrznaStatystykaUzytkownikaDTO(
    int ZewnetrzneIdUzytkownika,
    int StatystykaId,
    string Wartosc,
    int? PorownywalnaWartoscLiczbowa
);