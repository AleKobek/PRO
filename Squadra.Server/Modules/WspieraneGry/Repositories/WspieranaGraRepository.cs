using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Platformy.Models;
using Squadra.Server.Modules.WspieraneGry.DTO;
using Squadra.Server.Modules.WspieraneGry.Models;

namespace Squadra.Server.Modules.WspieraneGry.Repositories;

public class WspieranaGraRepository(AppDbContext context) : IWspieranaGraRepository
{
    public async Task<ICollection<WspieranaGra>> GetWspieraneGry()
    {
        return await context.WspieranaGra.ToListAsync();
    }
    
    public async Task<WspieranaGra> GetWspieranaGra(int id)
    {
        var gra = await context.WspieranaGra.FirstOrDefaultAsync(g => g.Id == id);
        if (gra is null)
            throw new NieZnalezionoWBazieException("Nie znaleziono gry o podanym id.");
        return gra;
    }
    
    public async Task<ICollection<WspieranaGra>> GetWspieraneGryMinInfo()
    {
        return await context.WspieranaGra.Select(g => new WspieranaGra
        {
            Id = g.Id,
            Tytul = g.Tytul,
            Wydawca = g.Wydawca,
            Gatunek = g.Gatunek
        }).ToListAsync();
    }
    
    public async Task<ICollection<Platforma>> GetPlatformyGry(int idGry)
    {
        var gra = await context.WspieranaGra.FirstOrDefaultAsync(g => g.Id == idGry);
        if (gra is null)
            throw new NieZnalezionoWBazieException("Nie znaleziono gry o podanym id.");
        
        var platformy = await context.GraNaPlatformie
            .Where(gp => gp.IdWspieranejGry == idGry)
            .Select(gp => gp.Platforma)
            .ToListAsync();
        
        return platformy;
    }
        
    public async Task<ICollection<GraZPlatformaDTO>> GetWspieraneGryZPlatformami()
    {
        var gry = await GetWspieraneGry();
        List<GraZPlatformaDTO> gryZPlatformami = new List<GraZPlatformaDTO>();
        foreach (var gra in gry)
        {
            gryZPlatformami.Add(new GraZPlatformaDTO(
                gra.Id,
                gra.Tytul,
                gra.Wydawca,
                gra.Gatunek,
                gra.GraNaPlatformieCollection.Select(gp => gp.Platforma).ToList()
            ));
        }
        return gryZPlatformami;
    }
}