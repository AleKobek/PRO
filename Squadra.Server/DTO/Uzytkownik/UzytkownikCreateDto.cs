namespace Squadra.Server.DTO.Uzytkownik;

public record UzytkownikCreateDto(
    int Id,
    string Login,
    string Haslo,
    string Email,
    string? NumerTelefonu,
    DateOnly DataUrodzenia,
    string Pseudonim
);