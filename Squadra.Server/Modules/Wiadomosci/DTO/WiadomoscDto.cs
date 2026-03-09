namespace Squadra.Server.DTO.Wiadomosc;

public record WiadomoscDto(
    int IdNadawcy,
    int IdOdbiorcy,
    string DataWyslania,
    string Tresc,
    int IdTypuWiadomosci
);