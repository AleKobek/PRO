using Squadra.Server.Modules.Profile.DTO.Profil;

namespace Squadra.Server.Modules.Drużyny.DTO;

public record MiejsceWDruzynieSzczegolyDto(
    int IdMiejscaWDruzynie,
    ProfilMinInfoDto? Czlonek,
    string? Rola,
    string? Wymaganie,
    bool CzyKapitan = false
);