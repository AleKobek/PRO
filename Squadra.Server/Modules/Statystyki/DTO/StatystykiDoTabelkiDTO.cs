namespace Squadra.Server.Modules.Statystyki.DTO;

public record StatystykiDoTabelkiDTO(
    int IdKategorii,
    string NazwaKategorii,
    ICollection<StatystykaDTO> Statystyki
);