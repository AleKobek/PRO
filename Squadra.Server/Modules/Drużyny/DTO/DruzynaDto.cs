namespace Squadra.Server.Modules.Drużyny.DTO;

public record DruzynaDto(
    int Id,
    string Nazwa,
    int GraId,
    int KapitanId,
    bool CzyPubliczna,
    string? Opis,
    int NastrojRozgrywkiId,
    int? WymaganyJezykId,
    int? WymaganyStopienBieglosciJezykaId,
    int? PlatformaId,
    bool CzyZintegrowano
);