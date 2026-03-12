using Squadra.Server.Modules.Profile.DTO.KrajRegion;

namespace Squadra.Server.Modules.Profile.Repositories;

public interface IRegionRepository
{
    public Task<ICollection<RegionDto>> GetRegiony();

    public Task<RegionDto?> GetRegion(int? id);

    public Task<ICollection<RegionDto>> GetRegionyKraju(int krajId);

    public Task<RegionKrajDto?> GetRegionIKraj(int? id);
    
}