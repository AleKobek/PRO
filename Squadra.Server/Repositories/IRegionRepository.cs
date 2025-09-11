

namespace Squadra;

public interface IRegionRepository
{
    public Task<ICollection<RegionDto>> GetRegiony();

    public Task<RegionDto?> GetRegion(int id);

    public Task<ICollection<RegionDto>> GetRegionyKraju(int krajId);

    public Region GetRegionDomyslny();

}