namespace Squadra.Server.Modules.Drużyny.DTO;

public record WyszukajDruzyneReqDto(
    int IdGry,
    int? IdPlatformy,
    int? IdNastrojuRozgrywki,
    int? IdJezyka,
    int? IdStopnia,
    bool CzyZintegrowano,
    string? Nazwa,
    int[] IdRol
);
