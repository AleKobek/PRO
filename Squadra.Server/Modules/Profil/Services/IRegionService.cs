using Squadra.Server.DTO.KrajRegion;

namespace Squadra.Server.Services;

public interface IRegionService
{
    public Task<ServiceResult<ICollection<RegionDto>>> GetRegiony();

    public Task<ServiceResult<RegionDto>> GetRegion(int id);

    public Task<ServiceResult<RegionKrajDto>> GetRegionIKraj(int id);

    public Task<ServiceResult<ICollection<RegionDto>>> GetRegionyKraju(int krajId);
}