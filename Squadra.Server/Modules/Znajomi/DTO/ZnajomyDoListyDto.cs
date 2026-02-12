namespace Squadra.Server.Modules.Znajomi.DTO;

public record ZnajomyDoListyDto(
    string Pseudonim,
    byte[] Awatar,
    DateTime? DataOstatniejWiadomosci,
    string NazwaStatusu,
    bool CzySaNoweWiadomosci
);