namespace Squadra.Server.Modules.BibliotekaGier.DTO;

// na przyszłość, jeszcze nie mamy statystyk
public record GraWBiblioteceDTO(
  int IdGry,
  string Tytuł,
  string Gatunek,
  int GodzinyGrania,
  ICollection<byte[]> LogaPlatform
);