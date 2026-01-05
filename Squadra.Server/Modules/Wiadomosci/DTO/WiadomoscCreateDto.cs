namespace Squadra.Server.DTO.Wiadomosc;

public record WiadomoscCreateDto(
    int IdOdbiorcy,
    string Tresc,
    int IdTypuWiadomosci
);