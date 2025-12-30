namespace Squadra.Server.DTO.Wiadomosc;

public record WiadomoscDto(
    int IdNadawcy,
    int IdOdbiorcy,
    DateTime DataWyslania,
    string Tresc,
    int IdTypuWiadomosci
);