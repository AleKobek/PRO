using Squadra.Server.Modules.Platformy.Models;

namespace Squadra.Server.Modules.WspieraneGry.DTO;

public record GraZPlatformaDTO(
    int Id,
    string Tytul,
    string Wydawca,
    string Gatunek,
    List<Platforma> Platformy
);