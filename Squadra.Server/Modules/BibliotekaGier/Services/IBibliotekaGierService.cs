using Squadra.Server.Modules.BibliotekaGier.DTO;
using Squadra.Server.Modules.BibliotekaGier.Models;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.BibliotekaGier.Services;

public interface IBibliotekaGierService
{
    public Task<ServiceResult<ICollection<GraWBiblioteceDTO>>> PodajGryWBiblioteceUzytkownika(int idUzytkownika);
    public Task<ServiceResult<bool>> UpdateBibliotekeGierUzytkownika(int idUzytkownika, List<GraUzytkownikaNaPlatformie> noweGryNaPlatformie, List<GraUzytkownika> noweGry);
    public Task<ServiceResult<bool>> WyczyscBibliotekeUzytkownika(int idUzytkownika);
}