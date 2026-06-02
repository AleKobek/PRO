namespace Squadra.Server.Modules.Drużyny.DTO;

public record WyszukajDruzynyResDto(
    int[] IdDruzyn,
    ICollection<DruzynaDoTabelkiDto> PierwszaStronaDruzyn
);