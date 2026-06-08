namespace Squadra.Server.Modules.Drużyny.DTO;

public record WyszukajDruzyneReqDto(
    int IdGry,
    int? IdPlatformy,
    int? IdNastrojuRozgrywki,
    int? IdJezyka,
    int? IdStopnia,
    string PreferencjeZintegrowania,
    string? Nazwa,
    int[] IdRol
);
