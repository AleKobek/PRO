using Squadra.Server.Modules.Statystyki.DTO;

namespace Squadra.Server.Modules.Drużyny.DTO;

public record CreateMiejsceWDruzynieReq(
    int IdDruzyny,
    int? IdUzytkownika,
    int? IdRoli,
    WartoscStatystykiDTO? WymaganaStatystyka
);