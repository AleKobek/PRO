namespace Squadra.Server.Modules.Drużyny.DTO;

public record DruzynaUpdateDto(
    string Nazwa,
    bool CzyPubliczna,
    string? Opis,
    int IdNastrojuRozgrywki,
    int? IdWymaganegoJezyka,
    int? IdWymaganegoStopniaBieglosciJezyka
);