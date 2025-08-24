using Microsoft.EntityFrameworkCore;
using Praca_Inzynierska.Context;
using Praca_Inzynierska.DTO;
using Praca_Inzynierska.Models;

namespace Praca_Inzynierska.Repositories;

public class RegionRepository : IRegionRepository
{
    private readonly AppDbContext _context;

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
    
    public async Task<ICollection<RegionDto>> GetRegionyKraju(int krajId)
    {
        ICollection<RegionDto> regionyDoZwrocenia = new List<RegionDto>();
        ICollection<Region> regiony = await _context.Region.ToListAsync();
        foreach (var region in regiony.Where(x => x.KrajId == krajId).ToList())
        {
            regionyDoZwrocenia.Add(new RegionDto(region.Id, region.KrajId,region.Nazwa));
        }

        return regionyDoZwrocenia;
    }
}