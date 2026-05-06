using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Profile.Models;

namespace Squadra.Server.Modules.Profile.Repositories;

public class StopienBieglosciJezykaRepository(AppDbContext appDbContext) : IStopienBieglosciJezykaRepository
{
    public async Task<ICollection<StopienBieglosciJezyka>> GetStopnieBieglosciJezyka()
    {
        return await appDbContext.StopienBieglosciJezyka.ToListAsync();
    }

    public async Task<StopienBieglosciJezyka> GetStopienBieglosciJezyka(int id)
    {
        var stopienBieglosci = await appDbContext.StopienBieglosciJezyka.FindAsync(id);
        if (stopienBieglosci == null) throw new Exception("Nie znaleziono stopnia bieglosci jezyka o id: " + id);
        return stopienBieglosci;
    }
}