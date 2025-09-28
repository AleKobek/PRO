using Microsoft.EntityFrameworkCore;
using Squadra.Exceptions;


namespace Squadra;

public class RegionRepository(AppDbContext context, IKrajRepository krajRepository) : IRegionRepository
{

    public async Task<ICollection<RegionDto>> GetRegiony()
    {
        ICollection<Region> regiony = await context.Region.ToListAsync();

        return regiony.Select(region => new RegionDto(region.Id, region.KrajId, region.Nazwa)).ToList();
        
    }

    // musi być ze znakami zapytania, bo profil repository wyrzuca błędy nawet jak staram się obchodzić nulla
    // bo może być możliwość proszenia o region nawet jeżeli jest nullem w profilu
    public async Task<RegionDto?> GetRegion(int? id)
    {
        if (id == null) return null;
        var region = await context.Region.FindAsync(id);
        if (region == null) throw new NieZnalezionoWBazieException("Region o id " + id + " nie istnieje");
        return new RegionDto(region.Id, region.KrajId,region.Nazwa);
    }

    // znaki zapytania z powodu takiego jak opisanego wyżej w komentarzach
    // bierzemy region oraz jego kraj
    public async Task<RegionKrajDto?> GetRegionIKraj(int? id)
    {
        if (id == null) return null;
        var region = await context.Region.FindAsync(id);
        // jeżeli miał być taki region, a i tak nie istnieje
        if (region == null) throw new NieZnalezionoWBazieException("Region o id " + id + " nie istnieje");
        var kraj = await krajRepository.GetKraj(region.KrajId);
        return new RegionKrajDto(region.Id, region.Nazwa, kraj.Id, kraj.Nazwa);
    }
    
    public async Task<ICollection<RegionDto>> GetRegionyKraju(int krajId)
    {
        ICollection<Region> regiony = await context.Region.Where(x => x.KrajId == krajId).ToListAsync();
        
        // nie rzucam wyjątkiem jeśli pusta, może będzie kraj bez regionów, bo np. taki mały

        return regiony.Select(region => new RegionDto(region.Id, region.KrajId, region.Nazwa)).ToList();
    }
    
}