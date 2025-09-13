using Microsoft.EntityFrameworkCore;


namespace Squadra;

public class RegionRepository(AppDbContext context, IKrajRepository krajRepository) : IRegionRepository
{

    public async Task<ICollection<RegionDto>> GetRegiony()
    {
        ICollection<Region> regiony = await context.Region.ToListAsync();

        return regiony.Select(region => new RegionDto(region.Id, region.KrajId, region.Nazwa)).ToList();
        
    }

    public async Task<RegionDto?> GetRegion(int? id)
    {
        if (id == null) return null;
        var region = await context.Region.FindAsync(id);
        return region != null ? new RegionDto(region.Id, region.KrajId,region.Nazwa) : null;
    }

    public async Task<RegionKrajDto?> GetRegionIKraj(int? id)
    {
        if (id == null) return null;
        var region = await context.Region.FindAsync(id);
        if (region == null) return null;
        var kraj = await krajRepository.GetKraj(region.KrajId ?? 1);
        return kraj == null ? 
            new RegionKrajDto(region.Id, region.Nazwa, null, null) 
            : new RegionKrajDto(region.Id, region.Nazwa, kraj.Id, kraj.Nazwa);
    }
    
    public async Task<ICollection<RegionDto>> GetRegionyKraju(int krajId)
    {
        ICollection<Region> regiony = await context.Region.Where(x => x.KrajId == krajId).ToListAsync();

        return regiony.Select(region => new RegionDto(region.Id, region.KrajId, region.Nazwa)).ToList();
    }
    
}