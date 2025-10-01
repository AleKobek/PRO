namespace Squadra.Server.DTO.Profil;

// potrzebny jest tylko pseudnim na start, podaje go przy rejestracji, reszta zaczyna pusta, a region jest ustawiany automatycznie na "nieznany"
public record ProfilCreateReqDto(
    int IdUzytkownika,
    string Pseudonim
);