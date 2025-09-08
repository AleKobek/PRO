
namespace Squadra;

// to jest po prostu get dto? nie, do czegoś jeszcze jest używane
public record UzytkownikDto
(
    int Id,
    string Login,
    string Haslo,
    string? NumerTelefonu,
    DateOnly? DataUrodzenia,
    int StatusId
);