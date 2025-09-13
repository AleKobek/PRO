namespace Squadra;

// nie trzeba tłumaczyć błędów, bo przy zmianie języka wszystko się odświeża (chyba)
public record ProfilUpdateBledyDto(
    string Pseudonim,
    string Zaimki,
    string Opis
    );