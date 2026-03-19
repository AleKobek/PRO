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
    
    public async Task<ServiceResult<Platforma>> GetPlatformaById(int id)
    {
        try
        {
            if(id < 1) return ServiceResult<Platforma>.NotFound(new ErrorItem("Platforma o id "+id+" nie istnieje."));
            
            return ServiceResult<Platforma>.Ok(await platformaRepository.GetPlatformaById(id));
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
}