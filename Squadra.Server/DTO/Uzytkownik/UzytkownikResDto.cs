
namespace Squadra;

// to jest po prostu get dto? nie, do czegoś jeszcze jest używane
public record UzytkownikResDto
(
    int Id,
    string Login,
    string Haslo,
    string Email,
    string? NumerTelefonu,
    DateOnly? DataUrodzenia,
    StatusDto Status
);