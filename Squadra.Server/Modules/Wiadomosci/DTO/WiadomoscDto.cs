namespace Squadra.Server.Modules.Wiadomosci.DTO;

public record WiadomoscDto(
    int IdNadawcy,
    int IdOdbiorcy,
    string DataWyslania,
    string Tresc,
    int IdTypuWiadomosci
);