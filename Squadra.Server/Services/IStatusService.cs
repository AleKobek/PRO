using Squadra.Server.DTO.Status;

namespace Squadra.Server.Services;

public interface IStatusService
{
    public Task<ServiceResult<ICollection<StatusDto>>> GetStatusy();

    public Task<ServiceResult<StatusDto?>> GetStatus(int id);
    
    public StatusDto GetStatusDomyslny();
}