using Squadra.Server.Modules.BibliotekaGier.DTO;
using Squadra.Server.Modules.BibliotekaGier.Models;

namespace Squadra.Server.Modules.BibliotekaGier.Repositories;

public interface IBibliotekaGierRepository
{
    public Task<ICollection<GraWBiblioteceDTO>> PodajGryWBiblioteceUzytkownika(int idUzytkownika);
    public Task<bool> UpdateBibliotekeGierUzytkownika(int idUzytkownika, List<GraUzytkownikaNaPlatformie> noweGryNaPlatformie, List<GraUzytkownika> noweGry);
    public Task<bool> WyczyscBibliotekeUzytkownika(int idUzytkownika);
}