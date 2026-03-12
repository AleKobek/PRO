namespace Squadra.Server.Modules.Uzytkownicy.DTO.Uzytkownik;

public record UzytkownikCreateDto(
    string Login,
    string Haslo,
    string Email,
    string? NumerTelefonu,
    DateOnly DataUrodzenia,
    string Pseudonim
);