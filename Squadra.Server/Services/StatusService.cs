using Squadra.Server.DTO.Status;
using Squadra.Server.Repositories;

namespace Squadra.Server.Services;

public class StatusService(IStatusRepository statusRepository) : IStatusService
{
    public async Task<ICollection<StatusDto>> GetStatusy()
    {
        return await statusRepository.GetStatusy();
    }
    
    public async Task<StatusDto?> GetStatus(int id)
    {
        return await statusRepository.GetStatus(id);
    }
    
    public StatusDto GetStatusDomyslny()
    {
        return statusRepository.GetStatusDomyslny();
    }
}