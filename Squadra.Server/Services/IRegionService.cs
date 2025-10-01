using Squadra.Server.DTO.KrajRegion;

namespace Squadra.Server.Services;

public interface IRegionService
{
    public Task<ICollection<RegionDto>> GetRegiony();

    public Task<RegionDto> GetRegion(int id);

    public Task<RegionKrajDto> GetRegionIKraj(int id);

    public Task<ICollection<RegionDto>> GetRegionyKraju(int krajId);
}