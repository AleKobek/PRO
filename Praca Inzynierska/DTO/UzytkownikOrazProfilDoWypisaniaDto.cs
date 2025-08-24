using Praca_Inzynierska.Models;

namespace Praca_Inzynierska.DTO;

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