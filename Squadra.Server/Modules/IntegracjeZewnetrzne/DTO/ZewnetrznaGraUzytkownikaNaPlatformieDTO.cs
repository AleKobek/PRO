namespace Squadra.Server.Modules.IntegracjeZewnetrzne.DTO;

public record ZewnetrznaGraUzytkownikaNaPlatformieDTO(
    int ZewnetrzneIdUzytkownika,
    int IdGry,
    int IdPlatformy
);