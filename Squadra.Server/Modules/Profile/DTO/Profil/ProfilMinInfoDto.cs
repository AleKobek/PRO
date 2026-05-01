namespace Squadra.Server.Modules.Profile.DTO.Profil;

public record ProfilMinInfoDto(
    int IdUzytkownika,
    string Pseudonim,
    byte[]? Awatar,
    string NazwaStatusu
);