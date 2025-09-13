namespace Squadra;

public record ProfilUpdateResDto(
    ProfilGetResDto? Profil,
    ProfilUpdateBledyDto Bledy,
    bool CzyPoprawne
    );