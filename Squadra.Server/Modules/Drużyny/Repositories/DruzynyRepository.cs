using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Drużyny.Models;
using Squadra.Server.Modules.Statystyki.Models;

namespace Squadra.Server.Modules.Drużyny.Repositories;

public class DruzynyRepository(AppDbContext context) : IDruzynyRepository
{
    
    public async Task<Druzyna> GetDruzyna(int idDruzyny)
    {
        var druzyna = await context.Druzyna.FindAsync(idDruzyny);
        if (druzyna == null) throw new NieZnalezionoWBazieException("Nie znaleziono drużyny o id " + idDruzyny);
        return druzyna;
    }
    
    public async Task<ICollection<MiejsceWDruzynie>> GetMiejscaWDruzynie(int idDruzyny)
    {
        var druzyna = await context.Druzyna.FindAsync(idDruzyny);
        if (druzyna == null) throw new NieZnalezionoWBazieException("Nie znaleziono drużyny o id " + idDruzyny);
        
        var miejscaWDruzynie = await context.MiejsceWDruzynie
            .Where(m => m.DruzynaId == idDruzyny)
            .Include(m => m.Rola)
            .ToListAsync();

        return miejscaWDruzynie;
    }
    
    public async Task<ICollection<NastrojRozgrywki>> GetNastrojeRozgrywki()
    {
        return await context.NastrojRozgrywki.ToListAsync();
    }
    
    public async Task<NastrojRozgrywki> GetNastrojRozgrywki(int idNastroju)
    {
        var nastroj = await context.NastrojRozgrywki.FindAsync(idNastroju);
        if (nastroj == null) throw new NieZnalezionoWBazieException("Nie znaleziono nastroju o id " + idNastroju);
        return nastroj;
    }
    
    public async Task<ICollection<Rola>> GetRoleGry(int idGry)
    {
        return await context.Rola.Where(r => r.IdGry == idGry).ToListAsync();
    }
}