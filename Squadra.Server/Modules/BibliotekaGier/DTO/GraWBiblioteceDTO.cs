using Squadra.Server.Modules.Platformy.DTO;

namespace Squadra.Server.Modules.BibliotekaGier.DTO;

public record GraWBiblioteceDTO(
  int IdGry,
  string Tytul,
  string Gatunek,
  int GodzinyGrania,
  ICollection<PlatformaWBiblioteceGierDTO> Platformy
);