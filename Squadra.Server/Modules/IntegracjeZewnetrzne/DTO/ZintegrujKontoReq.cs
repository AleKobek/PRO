namespace Squadra.Server.Modules.IntegracjeZewnetrzne.DTO;

public record ZintegrujKontoReq(
    string login,
    string haslo
);