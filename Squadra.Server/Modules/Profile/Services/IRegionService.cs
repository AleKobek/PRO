using Squadra.Server.Modules.Profile.DTO.KrajRegion;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Profile.Services;

public interface IRegionService
{
    public Task<ServiceResult<ICollection<RegionDto>>> GetRegiony();

    public Task<ServiceResult<RegionDto>> GetRegion(int id);

    public Task<ServiceResult<RegionKrajDto>> GetRegionIKraj(int id);

    public Task<ServiceResult<ICollection<RegionDto>>> GetRegionyKraju(int krajId);
}