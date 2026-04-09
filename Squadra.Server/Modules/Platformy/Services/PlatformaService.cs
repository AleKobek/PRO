using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Platformy.DTO;
using Squadra.Server.Modules.Platformy.Models;
using Squadra.Server.Modules.Platformy.Repositories;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Platformy.Services;

public class PlatformaService(IPlatformaRepository platformaRepository) : IPlatformaService
{
    public async Task<ServiceResult<ICollection<Platforma>>> GetPlatformy()
    {
        return ServiceResult<ICollection<Platforma>>.Ok(await platformaRepository.GetPlatformy());
    }
    
    public async Task<ServiceResult<Platforma>> GetPlatforma(int id)
    {
        try
        {
            if(id < 1) return ServiceResult<Platforma>.NotFound(new ErrorItem("Platforma o id "+id+" nie istnieje."));
            
            return ServiceResult<Platforma>.Ok(await platformaRepository.GetPlatforma(id));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<Platforma>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<ICollection<PlatformaUzytkownikaDTO>>> GetPlatformyUzytkownika(int idUzytkownika)
    {
        try
        {
            var platformy = await platformaRepository.GetPlatformyUzytkownika(idUzytkownika);
            return ServiceResult<ICollection<PlatformaUzytkownikaDTO>>.Ok(platformy);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<ICollection<PlatformaUzytkownikaDTO>>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<ICollection<PlatformaUzytkownikaDTO>>> UpdatePlatformyUzytkownika(int idUzytkownika)
    {
        try
        {
            if(idUzytkownika <= 0)
                return ServiceResult<ICollection<PlatformaUzytkownikaDTO>>.BadRequest(new ErrorItem("Podano nieprawidłowe id uzytkownika: " + idUzytkownika));
            var platformy = await platformaRepository.UpdatePlatformyUzytkownika(idUzytkownika);
            return ServiceResult<ICollection<PlatformaUzytkownikaDTO>>.Ok(platformy);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<ICollection<PlatformaUzytkownikaDTO>>.NotFound(new ErrorItem(e.Message));
        }
        catch(BrakIdNaZewnetrznymSerwisieException e)
        {
            return ServiceResult<ICollection<PlatformaUzytkownikaDTO>>.BadRequest(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> UsunPlatformyUzytkownika(int idUzytkownika)
    {
        try
        {
            var wynik = await platformaRepository.UsunPlatformyUzytkownika(idUzytkownika);
            return ServiceResult<bool>.Ok(wynik);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
}