using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Platformy.Models;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.WspieraneGry.DTO;
using Squadra.Server.Modules.WspieraneGry.Models;
using Squadra.Server.Modules.WspieraneGry.Repositories;

namespace Squadra.Server.Modules.WspieraneGry.Services;

public class WspieranaGraService(IWspieranaGraRepository wspieranaGraRepository) : IWspieranaGraService
{
    public async Task<ServiceResult<ICollection<WspieranaGra>>> GetWspieraneGry()
    {
        return ServiceResult<ICollection<WspieranaGra>>.Ok(await wspieranaGraRepository.GetWspieraneGry());
    }

    public async Task<ServiceResult<WspieranaGra>> GetWspieranaGra(int idGry)
    {
        try
        {
            if (idGry <= 0)
                return ServiceResult<WspieranaGra>.BadRequest(new ErrorItem("Nieprawidłowe id gry: " + idGry));
            return ServiceResult<WspieranaGra>.Ok(await wspieranaGraRepository.GetWspieranaGra(idGry));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<WspieranaGra>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<ICollection<WspieranaGra>>> GetWspieraneGryMinInfo()
    {
        return ServiceResult<ICollection<WspieranaGra>>.Ok(await wspieranaGraRepository.GetWspieraneGryMinInfo());
    }
    
    public async Task<ServiceResult<ICollection<GraZPlatformaDTO>>> GetWspieraneGryZPlatformami()
    {
        return ServiceResult<ICollection<GraZPlatformaDTO>>.Ok(await wspieranaGraRepository.GetWspieraneGryZPlatformami());
    }
    
    public async Task<ServiceResult<ICollection<Platforma>>> GetPlatformyGry(int idGry)
    {
        try
        {
            if (idGry <= 0)
                return ServiceResult<ICollection<Platforma>>.BadRequest(new ErrorItem("Nieprawidłowe id gry: " + idGry));
            return ServiceResult<ICollection<Platforma>>.Ok(await wspieranaGraRepository.GetPlatformyGry(idGry));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<ICollection<Platforma>>.NotFound(new ErrorItem(e.Message));
        }
    }
}