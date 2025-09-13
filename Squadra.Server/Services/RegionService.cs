namespace Squadra.Services;

public class RegionService(IRegionRepository regionRepository) : IRegionService
{
    public async Task<ICollection<RegionDto>> GetRegiony()
    {
        return await regionRepository.GetRegiony();
    }

    public async Task<RegionDto?> GetRegion(int? id)
    {
        return await regionRepository.GetRegion(id);
    }

    public async Task<RegionKrajDto?> GetRegionIKraj(int? id)
    {
        return await regionRepository.GetRegionIKraj(id);
    }

    public async Task<ICollection<RegionDto>> GetRegionyKraju(int krajId)
    {
        return await regionRepository.GetRegionyKraju(krajId);
    }

}