using Squadra.Exceptions;

namespace Squadra.Services;

public class RegionService(IRegionRepository regionRepository) : IRegionService
{
    public async Task<ICollection<RegionDto>> GetRegiony()
    {
        return await regionRepository.GetRegiony();
    }

    // tu już nie uwzględniamy nulla w int, bo null jest potrzebny tylko dla profil repository
    public async Task<RegionDto> GetRegion(int id)
    {
        var region = await regionRepository.GetRegion(id);
        if(region == null) throw new NieZnalezionoWBazieException("Region o id " + id + " nie istnieje");
        return region;
    }

    public async Task<RegionKrajDto> GetRegionIKraj(int id)
    {
        var region = await regionRepository.GetRegionIKraj(id);
        if(region == null) throw new NieZnalezionoWBazieException("Region o id " + id + " nie istnieje");
        return region;
    }

    public async Task<ICollection<RegionDto>> GetRegionyKraju(int krajId)
    {
        return await regionRepository.GetRegionyKraju(krajId);
    }

}