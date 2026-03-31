using Squadra.Server.Exceptions;
using Squadra.Server.Modules.BibliotekaGier.DTO;
using Squadra.Server.Modules.BibliotekaGier.Repositories;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.BibliotekaGier.Services;

public class BibliotekaGierService(IBibliotekaGierRepository bibliotekaGierRepository) : IBibliotekaGierService
{
    public async Task<ServiceResult<ICollection<GraWBiblioteceDTO>>> PodajGryWBiblioteceUzytkownika(int idUzytkownika)
    {
        try
        {
            if (idUzytkownika <= 0)
                return ServiceResult<ICollection<GraWBiblioteceDTO>>.BadRequest(new ErrorItem("Podano nieprawidłowe id uzytkownika: " + idUzytkownika));
            var listaGier = await bibliotekaGierRepository.PodajGryWBiblioteceUzytkownika(idUzytkownika);
            // tutaj będzie podmiana 0 w godzinach rozgrywki na prawdziwe statystyki, ale to gdy zrobię moduł statystyk
            return ServiceResult<ICollection<GraWBiblioteceDTO>>.Ok(listaGier);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<ICollection<GraWBiblioteceDTO>>.NotFound(new ErrorItem(e.Message));
        }
    }
}