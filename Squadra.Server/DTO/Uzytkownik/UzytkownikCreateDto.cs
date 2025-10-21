namespace Squadra.Server.DTO.Uzytkownik;

public record UzytkownikCreateDto(
    string Login,
    string HasloHashed,
    string Email,
    string? NumerTelefonu,
    DateOnly DataUrodzenia,
    string Pseudonim
);