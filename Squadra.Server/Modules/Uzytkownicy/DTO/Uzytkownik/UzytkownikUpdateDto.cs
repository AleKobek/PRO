namespace Squadra.Server.Modules.Uzytkownicy.DTO.Uzytkownik;

public record UzytkownikUpdateDto(
    string Login,
    string Email,
    string? NumerTelefonu,
    DateOnly DataUrodzenia
);