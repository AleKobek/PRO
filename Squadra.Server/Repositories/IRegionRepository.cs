

using Squadra.Server.DTO.KrajRegion;

namespace Squadra.Server.Repositories;

public interface IRegionRepository
{
    public Task<ICollection<RegionDto>> GetRegiony();

    public Task<RegionDto?> GetRegion(int? id);

    public Task<ICollection<RegionDto>> GetRegionyKraju(int krajId);

    public Task<RegionKrajDto?> GetRegionIKraj(int? id);
    
}