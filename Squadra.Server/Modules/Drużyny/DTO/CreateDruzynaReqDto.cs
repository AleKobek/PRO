using Squadra.Server.Modules.Statystyki.DTO;

namespace Squadra.Server.Modules.Drużyny.DTO;

public record CreateDruzynaReqDto(
    string Nazwa,
    int IdGry,
    bool CzyPubliczna,
    string? Opis,
    int IdNastrojuRozgrywki,
    int? IdWymaganegoJezyka,
    int? IdWymaganegoStopniaBieglosciJezyka,
    int? IdPlatformy,
    int? IdRoliKapitana,
    ICollection<WartoscStatystykiDTO>? WymaganeStatystyki,
    ICollection<CreateMiejsceWDruzynieReq> MiejscaWDruzynie
);