using Squadra.Server.Modules.Statystyki.DTO;

namespace Squadra.Server.Modules.Drużyny.DTO;

public record CreateMiejsceWDruzynieReq(
    int? IdRoli,
    WartoscStatystykiDTO? WymaganaStatystyka
);