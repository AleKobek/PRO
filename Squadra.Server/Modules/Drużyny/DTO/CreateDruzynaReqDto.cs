using Squadra.Server.Modules.Drużyny.Models;

namespace Squadra.Server.Modules.Drużyny.DTO;

public record CreateDruzynaReqDto(
    string Nazwa,
    int IdGry,
    bool CzyPubliczna,
    string? Opis,
    int? IdNastrojuRozgrywki,
    int? IdWymaganegoJezyka,
    int? IdWymaganegoStopniaBieglosciJezyka,
    bool Czy18Plus,
    int? IdPlatformy,
    ICollection<WymaganaStatystykaDruzyny>? WymaganeStatystyki,
    ICollection<CreateMiejsceWDruzynieReq> MiejscaWDruzynie
);