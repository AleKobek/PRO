using Squadra.Server.Modules.Profile.DTO.Status;
using Squadra.Server.Modules.Profile.Repositories;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Profile.Services;

public class StatusyService(IStatusyRepository statusyRepository) : IStatusyService
{
    public async Task<ServiceResult<ICollection<StatusDto>>> GetStatusy()
    {
        return ServiceResult<ICollection<StatusDto>>.Ok(await statusyRepository.GetStatusy());
    }
    
    public async Task<ServiceResult<StatusDto?>> GetStatus(int id)
    {
        var status = await statusyRepository.GetStatus(id);
        return status == null ? ServiceResult<StatusDto?>.NotFound(new ErrorItem("Status o id " + id + " nie istnieje")) : ServiceResult<StatusDto?>.Ok(status);
    }
}