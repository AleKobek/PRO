using Squadra.Server.DTO.KrajRegion;
using Squadra.Server.Exceptions;
using Squadra.Server.Repositories;

namespace Squadra.Server.Services;

public class KrajService (IKrajRepository krajRepository) : IKrajService
{
    public async Task<ServiceResult<ICollection<KrajDto>>> GetKraje()
    {
        return ServiceResult<ICollection<KrajDto>>.Ok(await krajRepository.GetKraje());
    }

    public async Task<ServiceResult<KrajDto>> GetKraj(int id)
    {
        try
        {
            return id < 1
                ? ServiceResult<KrajDto>.NotFound(new ErrorItem("Kraj o id " + id + " nie istnieje"))
                : ServiceResult<KrajDto>.Ok(await krajRepository.GetKraj(id));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<KrajDto>.NotFound(new ErrorItem(e.Message));
        }
    }
}