using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Modules.Platformy.Models;

namespace Squadra.Server.Modules.Platformy.Repositories;

public class PlatformaRepository(AppDbContext context) : IPlatformaRepository{
    
    public async Task<ICollection<Platforma>> GetPlatformy()
    {
        return await context.Platforma.ToListAsync();
    }
    
    public async Task<Platforma?> GetPlatformaById(int id)
    {
        return await context.Platforma.FirstOrDefaultAsync(p => p.Id == id);
    }
}