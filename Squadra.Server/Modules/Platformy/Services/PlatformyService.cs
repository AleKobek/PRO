using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Platformy.DTO;
using Squadra.Server.Modules.Platformy.Models;
using Squadra.Server.Modules.Platformy.Repositories;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Platformy.Services;

public class PlatformyService(IPlatformyRepository platformyRepository) : IPlatformyService
{
    public async Task<ServiceResult<ICollection<PlatformaDto>>> GetPlatformy()
    {
        var platformy = await platformyRepository.GetPlatformy();
        return ServiceResult<ICollection<PlatformaDto>>.Ok(platformy.Select(p => new PlatformaDto(p.Id, p.Nazwa, p.Logo)).ToList());
    }
    
    public async Task<ServiceResult<PlatformaDto>> GetPlatforma(int id)
    {
        try
        {
            if(id < 1) return ServiceResult<PlatformaDto>.BadRequest(new ErrorItem("Nieprawidłowe id platformy: " + id));
            
            var platforma = await platformyRepository.GetPlatforma(id);
            
            return ServiceResult<PlatformaDto>.Ok(new PlatformaDto(platforma.Id, platforma.Nazwa, platforma.Logo));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<PlatformaDto>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<ICollection<PlatformaUzytkownikaDTO>>> GetPlatformyUzytkownika(int idUzytkownika)
    {
        try
        {
            if(idUzytkownika <= 0)
                return ServiceResult<ICollection<PlatformaUzytkownikaDTO>>.BadRequest(new ErrorItem("Podano nieprawidłowe id uzytkownika: " + idUzytkownika));
            var platformy = await platformyRepository.GetPlatformyUzytkownika(idUzytkownika);
            return ServiceResult<ICollection<PlatformaUzytkownikaDTO>>.Ok(platformy);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<ICollection<PlatformaUzytkownikaDTO>>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> UpdatePlatformyUzytkownika(int idUzytkownika, List<UzytkownikPlatforma> nowePlatformy)
    {
        try
        {
            if(idUzytkownika <= 0)
                return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id uzytkownika: " + idUzytkownika));
            var result = await platformyRepository.UpdatePlatformyUzytkownika(idUzytkownika, nowePlatformy);
            return ServiceResult<bool>.Ok(result);
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
    
    public async Task<ServiceResult<bool>> UsunPlatformyUzytkownika(int idUzytkownika)
    {
        try
        {
            if(idUzytkownika <= 0)
                return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id uzytkownika: " + idUzytkownika));
            var wynik = await platformyRepository.UsunPlatformyUzytkownika(idUzytkownika);
            return ServiceResult<bool>.Ok(wynik);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
    
}