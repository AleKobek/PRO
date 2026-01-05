using Squadra.Server.DTO.Status;

namespace Squadra.Server.DTO.Uzytkownik;

public record UzytkownikUpdateDto(
    string Login,
    string Email,
    string? NumerTelefonu,
    DateOnly DataUrodzenia
);