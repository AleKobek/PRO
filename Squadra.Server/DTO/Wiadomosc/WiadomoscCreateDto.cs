namespace Squadra.Server.DTO.Wiadomosc;

public record WiadomoscCreateDto(
    int IdNadawcy,
    int IdOdbiorcy,
    string Tresc,
    int IdTypuWiadomosci
);