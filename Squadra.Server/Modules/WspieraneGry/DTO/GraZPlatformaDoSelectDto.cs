namespace Squadra.Server.Modules.WspieraneGry.DTO;

public record GraZPlatformaDoSelectDto(
    int Id,
    string Tytul,
    List<PlatformaMinInfo> Platformy
);