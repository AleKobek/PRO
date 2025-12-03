namespace Squadra.Server.DTO.Powiadomienie;

public record OdpowiedzNaPowiadomienieDto(
    int IdPowiadomienia,
    bool? CzyZaakceptowane // przy tych bez reakcji to null, bo się tylko usuwa
);