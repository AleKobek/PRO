namespace Squadra.Server.Modules.Profile.DTO.Profil;

public record ProfilUpdateResDto(
    ProfilGetResDto? Profil,
    ProfilUpdateBledyDto Bledy,
    bool CzyPoprawne
    );