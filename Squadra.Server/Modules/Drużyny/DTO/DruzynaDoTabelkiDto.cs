namespace Squadra.Server.Modules.Drużyny.DTO;

public record DruzynaDoTabelkiDto(
    int Id,
    string Nazwa,
    string TytulGry,
    string OstatniaAktywnoscKapitana, // string w formacie "X dni Y godzin Z minut temu"
    int MinutyOdOstatniejAktywnosciKapitana, // do porównywania
    ICollection<MiejsceWDruzynieWTabelceDto> Czlonkowie,
    string NazwaNastroju
);