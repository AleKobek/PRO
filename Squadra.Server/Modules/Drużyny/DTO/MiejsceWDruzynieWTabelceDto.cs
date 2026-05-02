using Squadra.Server.Modules.Profile.DTO.Profil;

namespace Squadra.Server.Modules.Drużyny.DTO;

public record MiejsceWDruzynieWTabelceDto(
    ProfilMinInfoDto? Czlonek,
    string? Rola,
    bool CzyKapitan = false
);