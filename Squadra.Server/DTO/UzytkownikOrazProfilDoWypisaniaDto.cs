

namespace Squadra;

public record UzytkownikOrazProfilDoWypisaniaDto
(
    int Id,
    string Login,
    string Pseudonim,
    string Haslo,
    string Region,
    string? NumerTelefonu,
    string? Zaimki,
    string? Opis,
    ICollection<JezykOrazStopienDoWypisaniaDto> Jezyki
);