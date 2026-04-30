using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Statystyki.DTO;
using Squadra.Server.Modules.Statystyki.Models;
using Squadra.Server.Modules.Statystyki.Repositories;

namespace Squadra.Server.Modules.Statystyki.Services;

public class StatystykiService(IStatystykiRepository statystykiRepository) : IStatystykiService
{
    public async Task<ServiceResult<string>> GetGodzinyGrania(int idUzytkownika, int idGry)
    {
        if (idUzytkownika <= 0)
        {
            return ServiceResult<string>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator użytkownika: " + idUzytkownika));
        }
        if (idGry <= 0)
        {
            return ServiceResult<string>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator gry: " + idGry));
        }
        
        try
        {
            var result = await statystykiRepository.GetGodzinyGrania(idUzytkownika, idGry);
            return ServiceResult<string>.Ok(result);
        }
        catch (NieZnalezionoWBazieException ex)
        {
            return ServiceResult<string>.NotFound(new ErrorItem(ex.Message));
        }
    }
    
    public async Task<ServiceResult<ICollection<CzasRozgrywkiDTO>>> GetGodzinyGraniaUzytkownika(int idUzytkownika)
    {
        if (idUzytkownika <= 0)
        {
            return ServiceResult<ICollection<CzasRozgrywkiDTO>>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator użytkownika: " + idUzytkownika));
        }
        
        try
        {
            var result = await statystykiRepository.GetGodzinyGraniaUzytkownika(idUzytkownika);
            return ServiceResult<ICollection<CzasRozgrywkiDTO>>.Ok(result);
        }
        catch (NieZnalezionoWBazieException ex)
        {
            return ServiceResult<ICollection<CzasRozgrywkiDTO>>.NotFound(new ErrorItem(ex.Message));
        }
    }
    
    public async Task<ServiceResult<WartoscStatystykiDTO?>> GetWartoscStatystyki(int idUzytkownika, int idStatystyki)
    {
        if (idUzytkownika <= 0)
        {
            return ServiceResult<WartoscStatystykiDTO?>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator użytkownika: " + idUzytkownika));
        }
        if (idStatystyki <= 0)
        {
            return ServiceResult<WartoscStatystykiDTO?>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator statystyki: " + idStatystyki));
        }
        
        try
        {
            var result = await statystykiRepository.GetWartoscStatystyki(idUzytkownika, idStatystyki);
            return ServiceResult<WartoscStatystykiDTO?>.Ok(result);
        }
        catch (NieZnalezionoWBazieException ex)
        {
            return ServiceResult<WartoscStatystykiDTO?>.NotFound(new ErrorItem(ex.Message));
        }
    }
    
    public async Task<ServiceResult<ICollection<StatystykiDoTabelkiDTO>>> GetStatystykiZGry(int idUzytkownika, int idGry)
    {
        if (idUzytkownika <= 0)
        {
            return ServiceResult<ICollection<StatystykiDoTabelkiDTO>>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator użytkownika: " + idUzytkownika));
        }
        if (idGry <= 0)
        {
            return ServiceResult<ICollection<StatystykiDoTabelkiDTO>>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator gry: " + idGry));
        }
        
        try
        {
            var result = await statystykiRepository.GetStatystykiZGry(idUzytkownika, idGry);
            return ServiceResult<ICollection<StatystykiDoTabelkiDTO>>.Ok(result);
        }
        catch (NieZnalezionoWBazieException ex)
        {
            return ServiceResult<ICollection<StatystykiDoTabelkiDTO>>.NotFound(new ErrorItem(ex.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> UpdateStatystykiUzytkownika(int idUzytkownika, List<StatystykaUzytkownika> noweStatystyki)
    {
        if (idUzytkownika <= 0)
        {
            return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator użytkownika: " + idUzytkownika));
        }
        
        try
        {
            var result = await statystykiRepository.UpdateStatystykiUzytkownika(idUzytkownika, noweStatystyki);
            return ServiceResult<bool>.Ok(result);
        }
        catch (NieZnalezionoWBazieException ex)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(ex.Message));
        }catch(BrakIdNaZewnetrznymSerwisieException e)
        {
            return ServiceResult<bool>.BadRequest(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> UsunStatystykiUzytkownika(int idUzytkownika)
    {
        if (idUzytkownika <= 0)
        {
            return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator użytkownika: " + idUzytkownika));
        }
        
        try
        {
            var result = await statystykiRepository.UsunStatystykiUzytkownika(idUzytkownika);
            return ServiceResult<bool>.Ok(result);
        }
        catch (NieZnalezionoWBazieException ex)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(ex.Message));
        }
    }

    public ServiceResult<bool> CzySpelniaWymagania(ICollection<WartoscStatystykiDTO> wymagania, ICollection<WartoscStatystykiDTO> statystykiDoSprawdzenia)
    {
        var czySaBledneIdentyfikatoryWWymaganiach = wymagania.Any(x => x.IdStatystyki <= 0);
        if (czySaBledneIdentyfikatoryWWymaganiach){
            return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator statystyki w wymaganiach."));
        }
        var czySaBledneIdentyfikatoryWStatystykachDoSprawdzenia = statystykiDoSprawdzenia.Any(x => x.IdStatystyki <= 0);
        if (czySaBledneIdentyfikatoryWStatystykachDoSprawdzenia){
            return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator statystyki w statystykach do sprawdzenia ."));
        }
        var result = statystykiRepository.CzySpelniaWymagania(wymagania, statystykiDoSprawdzenia);
        return ServiceResult<bool>.Ok(result);
    }
}