using Squadra.Server.Modules.Profile.DTO.KrajRegion;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Profile.Services;

public interface IKrajService
{
    public Task<ServiceResult<ICollection<KrajDto>>> GetKraje();

    public Task<ServiceResult<KrajDto>> GetKraj(int id);
}