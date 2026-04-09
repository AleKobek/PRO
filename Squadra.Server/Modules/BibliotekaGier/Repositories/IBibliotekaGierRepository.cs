using Squadra.Server.Modules.BibliotekaGier.DTO;

namespace Squadra.Server.Modules.BibliotekaGier.Repositories;

public interface IBibliotekaGierRepository
{
    public Task<ICollection<GraWBiblioteceDTO>> PodajGryWBiblioteceUzytkownika(int idUzytkownika);
    public Task<bool> UpdateBibliotekeGierUzytkownika(int idUzytkownika);
    public Task<bool> WyczyscBibliotekeUzytkownika(int idUzytkownika);
}