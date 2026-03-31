using Squadra.Server.Modules.BibliotekaGier.DTO;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.BibliotekaGier.Services;

public interface IBibliotekaGierService
{
    public Task<ServiceResult<ICollection<GraWBiblioteceDTO>>> PodajGryWBiblioteceUzytkownika(int idUzytkownika);

}