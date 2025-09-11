using Microsoft.EntityFrameworkCore;


namespace Squadra;

public class RegionRepository(AppDbContext context, IKrajRepository krajRepository) : IRegionRepository
{

    public async Task<ICollection<RegionDto>> GetRegiony()
    {
        ICollection<RegionDto> regionyDoZwrocenia = new List<RegionDto>();
        ICollection<Region> regiony = await context.Region.ToListAsync();
        foreach (var region in regiony)
        {
            regionyDoZwrocenia.Add(new RegionDto(region.Id, region.KrajId,region.Nazwa));
        }

        return regionyDoZwrocenia;
    }

    public async Task<RegionDto?> GetRegion(int id)
    {
        Region? region = await context.Region.FindAsync(id);
        return region != null ? new RegionDto(region.Id, region.KrajId,region.Nazwa) : null;
    }
    
    public async Task<ICollection<RegionDto>> GetRegionyKraju(int krajId)
    {
        ICollection<RegionDto> regionyDoZwrocenia = new List<RegionDto>();
        ICollection<Region> regiony = await context.Region.Where(x => x.KrajId == krajId).ToListAsync();
        foreach (var region in regiony)
        {
            regionyDoZwrocenia.Add(new RegionDto(region.Id, region.KrajId,region.Nazwa));
        }

        return regionyDoZwrocenia;
    }

    public Region GetRegionDomyslny()
    {
        
        return new Region
        {
            Id = 1,
            KrajId = 1,
            Nazwa = "Unknown"
        };
    }
}