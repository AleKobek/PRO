using Squadra.Server.Modules.Statystyki.DTO;

namespace Squadra.Server.Modules.Drużyny.DTO;

public record DruzynaSzczegolyDto(
    string Nazwa,
    string TytulGry,
    string? Opis,
    string? NastrojRozgrywki,
    ICollection<MiejsceWDruzynieSzczegolyDto> Czlonkowie,
    string? WymaganyJezykIStopienBiegłosci,
    ICollection<WymaganieDruzynyDoWyswietleniaDto> WymaganiaDoWypisania,
    bool CzyPubliczna,
    bool Czy18Plus,
    string? NazwaPlatformy,
    byte[]? LogoPlatformy
);