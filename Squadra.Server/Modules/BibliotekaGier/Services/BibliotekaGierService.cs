using Squadra.Server.Exceptions;
using Squadra.Server.Modules.BibliotekaGier.DTO;
using Squadra.Server.Modules.BibliotekaGier.Models;
using Squadra.Server.Modules.BibliotekaGier.Repositories;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Statystyki.DTO;
using Squadra.Server.Modules.Statystyki.Services;

namespace Squadra.Server.Modules.BibliotekaGier.Services;

public class BibliotekaGierService(IBibliotekaGierRepository bibliotekaGierRepository,
                                    IStatystykiService statystykiService) : IBibliotekaGierService
{
    public async Task<ServiceResult<ICollection<GraWBiblioteceDTO>>> PodajGryWBiblioteceUzytkownika(int idUzytkownika)
    {
        try
        {
            if (idUzytkownika <= 0)
                return ServiceResult<ICollection<GraWBiblioteceDTO>>.BadRequest(new ErrorItem("Podano nieprawidłowe id uzytkownika: " + idUzytkownika));
            var listaGier = await bibliotekaGierRepository.PodajGryWBiblioteceUzytkownika(idUzytkownika);
            var listaCzasowGierRes = await statystykiService.GetGodzinyGraniaUzytkownika(idUzytkownika);
            var listaCzasowGier = listaCzasowGierRes.Value ?? new List<CzasRozgrywkiDTO>();
            // nie wywali wyjątków, bo już sprawdziliśmy, że użytkownik istnieje, ale może zwrócić pustą listę, jeśli użytkownik nie ma żadnych gier w bibliotece.
            // to jest ok, bo wtedy po prostu zwrócimy pustą listę gier, a nie błąd 404
            
            var listaGierDoZwrocenia = listaGier
                .Select(g => 
                    g with { GodzinyGrania = listaCzasowGier.FirstOrDefault(x => x.IdGry == g.IdGry)?.GodzinyGrania ?? 0 })
                .ToList();

            return ServiceResult<ICollection<GraWBiblioteceDTO>>.Ok(listaGierDoZwrocenia);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<ICollection<GraWBiblioteceDTO>>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> UpdateBibliotekeGierUzytkownika(
        int idUzytkownika, List<GraUzytkownikaNaPlatformie> noweGryNaPlatformie, List<GraUzytkownika> noweGry
    ){
        try
        {
            if (idUzytkownika <= 0)
                return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id uzytkownika: " + idUzytkownika));
            var wynik = await bibliotekaGierRepository.UpdateBibliotekeGierUzytkownika(idUzytkownika, noweGryNaPlatformie, noweGry);
            return ServiceResult<bool>.Ok(wynik);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
        catch(BrakIdNaZewnetrznymSerwisieException e)
        {
            return ServiceResult<bool>.BadRequest(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> WyczyscBibliotekeUzytkownika(int idUzytkownika)
    {
        try
        {
            if (idUzytkownika <= 0)
                return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id uzytkownika: " + idUzytkownika));
            var wynik = await bibliotekaGierRepository.WyczyscBibliotekeUzytkownika(idUzytkownika);
            return ServiceResult<bool>.Ok(wynik);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
}