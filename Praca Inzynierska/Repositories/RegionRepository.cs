using Microsoft.EntityFrameworkCore;
using Praca_Inzynierska.Context;
using Praca_Inzynierska.DTO;
using Praca_Inzynierska.Models;

namespace Praca_Inzynierska.Repositories;

public class RegionRepository : IRegionRepository
{
    private readonly AppDbContext _context;
    private readonly IKrajRepository _krajRepository;

    public RegionRepository(AppDbContext appDbContext)
    {
        _context = appDbContext;
    }


    public async Task<ICollection<RegionDto>> GetRegiony()
    {
        ICollection<RegionDto> regionyDoZwrocenia = new List<RegionDto>();
        ICollection<Region> regiony = await _context.Region.ToListAsync();
        foreach (var region in regiony)
        {
            regionyDoZwrocenia.Add(new RegionDto(region.Id, region.KrajId,region.Nazwa));
        }

        return regionyDoZwrocenia;
    }

    public async Task<RegionDto?> GetRegion(int id)
    {
        Region? region = await _context.Region.FindAsync(id);
        return region != null ? new RegionDto(region.Id, region.KrajId,region.Nazwa) : null;
    }
    
    public async Task<ICollection<RegionDto>> GetRegionyKraju(int krajId)
    {
        ICollection<RegionDto> regionyDoZwrocenia = new List<RegionDto>();
        ICollection<Region> regiony = await _context.Region.Where(x => x.KrajId == krajId).ToListAsync();
        foreach (var region in regiony)
        {
            regionyDoZwrocenia.Add(new RegionDto(region.Id, region.KrajId,region.Nazwa));
        }

        return regionyDoZwrocenia;
    }
}