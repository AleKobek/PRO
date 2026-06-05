namespace Squadra.Server.Modules.Drużyny.DTO;

public record TabelkaDruzynResDto(
    int[] IdDruzyn,
    ICollection<DruzynaDoTabelkiDto> PierwszaStronaDruzyn
);