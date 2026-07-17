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
    // zwraca wszystkie gry, które użytkownik ma w swojej bibliotece w formacie do tabelki na stronie profilu
    public async Task<ServiceResult<ICollection<GraWBiblioteceDTO>>> GetGryWBiblioteceUzytkownika(int idUzytkownika)
    {
        try
        {
            if (idUzytkownika <= 0)
                return ServiceResult<ICollection<GraWBiblioteceDTO>>.BadRequest(new ErrorItem("Podano nieprawidłowe id uzytkownika: " + idUzytkownika));
            var listaGier = await bibliotekaGierRepository.GetGryWBiblioteceUzytkownika(idUzytkownika);
            var listaCzasowGierRes = await statystykiService.GetGodzinyGraniaUzytkownika(idUzytkownika);
            var listaCzasowGier = listaCzasowGierRes.Value ?? new List<CzasRozgrywkiDTO>();
            
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

    public async Task<ServiceResult<bool>> CzyUzytkownikMaDanaGreNaDanejPlatformie(int idUzytkownika, int idGry, int idPlatformy)
    {
        if(idUzytkownika <= 0)
            return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id uzytkownika: " + idUzytkownika));
        if(idGry <= 0)            
            return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id gry: " + idGry));
        if(idPlatformy <= 0)            
            return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id platformy: " + idPlatformy));
        try
        {
            var wynik = await bibliotekaGierRepository.CzyUzytkownikMaDanaGreNaDanejPlatformie(idUzytkownika, idGry, idPlatformy);
            return ServiceResult<bool>.Ok(wynik);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> CzyUzytkownikMaDanaGre(int idUzytkownika, int idGry)
    {
        if(idUzytkownika <= 0)
            return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id uzytkownika: " + idUzytkownika));
        if(idGry <= 0)            
            return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id gry: " + idGry));
        try
        {
            var wynik = await bibliotekaGierRepository.CzyUzytkownikMaDanaGre(idUzytkownika, idGry);
            return ServiceResult<bool>.Ok(wynik);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }

    // funkcja aktualizująca gry w bibliotece użytkownika, wywoływana przy pomyślnym "łączeniu kont z zewnętrznym serwisem", aby użytkownik nie musiał czekać do automatycznej aktualizacji
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
    
    // czyści bibliotekę użytkownika, czyli wszystkie jego dane z tabel: GraUzytkownikaNaPlatformie, GraUzytkownika
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