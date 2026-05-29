using Squadra.Server.Modules.Statystyki.DTO;

namespace Squadra.Server.Modules.Drużyny.DTO;

public record DruzynaSzczegolyDto(
    string Nazwa,
    string TytulGry,
    string? Opis,
    string? NastrojRozgrywki,
    ICollection<MiejsceWDruzynieSzczegolyDto> Czlonkowie,
    string? WymaganyJezykIStopienBieglosci,
    ICollection<WymaganieDruzynyDoWyswietleniaDto> WymaganiaDoWypisania,
    bool CzyPubliczna,
    string? NazwaPlatformy,
    byte[]? LogoPlatformy,
    string StatusCzlonkostwa
);