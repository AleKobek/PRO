namespace Squadra.Server.DTO.Profil;

public record ProfilUpdateResDto(
    ProfilGetResDto? Profil,
    ProfilUpdateBledyDto Bledy,
    bool CzyPoprawne
    );