using Squadra.Server.DTO.Status;

namespace Squadra.Server.DTO.Uzytkownik;

public record UzytkownikUpdateDto(
    int Id,
    string Login,
    string Haslo,
    string Email,
    string? NumerTelefonu,
    DateOnly DataUrodzenia,
    StatusDto Status
);