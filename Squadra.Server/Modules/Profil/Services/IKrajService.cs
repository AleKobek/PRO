using Squadra.Server.DTO.KrajRegion;

namespace Squadra.Server.Services;

public interface IKrajService
{
    public Task<ServiceResult<ICollection<KrajDto>>> GetKraje();

    public Task<ServiceResult<KrajDto>> GetKraj(int id);
}