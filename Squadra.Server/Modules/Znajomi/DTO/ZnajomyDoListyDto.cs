namespace Squadra.Server.Modules.Znajomi.DTO;

public record ZnajomyDoListyDto(
    int IdZnajomego,
    string Pseudonim,
    byte[] Awatar,
    DateTime? DataOstatniejWiadomosci,
    string NazwaStatusu,
    bool CzySaNoweWiadomosci
);