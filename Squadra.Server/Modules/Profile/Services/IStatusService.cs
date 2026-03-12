using Squadra.Server.Modules.Profile.DTO.Status;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Profile.Services;

public interface IStatusService
{
    public Task<ServiceResult<ICollection<StatusDto>>> GetStatusy();

    public Task<ServiceResult<StatusDto?>> GetStatus(int id);
    
    public StatusDto GetStatusDomyslny();
}