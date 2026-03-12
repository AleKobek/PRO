namespace Squadra.Server.Modules.Znajomosci.DTO;

public record ZnajomyDoListyDto(
    int IdZnajomego,
    string Pseudonim,
    byte[] Awatar,
    DateTime? DataOstatniejWiadomosci,
    string NazwaStatusu,
    bool CzySaNoweWiadomosci
);