using Squadra.Server.DTO.Status;
using Squadra.Server.Repositories;

namespace Squadra.Server.Services;

public class StatusService(IStatusRepository statusRepository) : IStatusService
{
    public async Task<ServiceResult<ICollection<StatusDto>>> GetStatusy()
    {
        return ServiceResult<ICollection<StatusDto>>.Ok(await statusRepository.GetStatusy());
    }
    
    public async Task<ServiceResult<StatusDto?>> GetStatus(int id)
    {
        var status = await statusRepository.GetStatus(id);
        return status == null ? ServiceResult<StatusDto?>.NotFound(new ErrorItem("Status o id " + id + " nie istnieje")) : ServiceResult<StatusDto?>.Ok(status);
    }
    
    public StatusDto GetStatusDomyslny()
    {
        return statusRepository.GetStatusDomyslny();
    }
}