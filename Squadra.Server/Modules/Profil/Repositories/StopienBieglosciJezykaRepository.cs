using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.DTO.JezykStopien;
using Squadra.Server.Models;

namespace Squadra.Server.Repositories;

public class StopienBieglosciJezykaRepository(AppDbContext appDbContext) : IStopienBieglosciJezykaRepository
{
    public async Task<ICollection<StopienBieglosciJezykaDto>> GetStopnieBieglosciJezyka()
    {
        ICollection<StopienBieglosciJezykaDto> stopnieBieglosciDoZwrocenia = new List<StopienBieglosciJezykaDto>();
        ICollection<StopienBieglosciJezyka> stopnieBieglosci = await appDbContext.StopienBieglosciJezyka.ToListAsync();
        foreach (var stopienBieglosci in stopnieBieglosci)
        {
            stopnieBieglosciDoZwrocenia.Add(new StopienBieglosciJezykaDto(stopienBieglosci.Id, stopienBieglosci.Nazwa, stopienBieglosci.Wartosc));
        }

        return stopnieBieglosciDoZwrocenia;
    }

    public async Task<StopienBieglosciJezykaDto?> GetStopienBieglosciJezyka(int id)
    {
        var stopienBieglosci = await appDbContext.StopienBieglosciJezyka.FindAsync(id);
        return stopienBieglosci != null ? new StopienBieglosciJezykaDto(stopienBieglosci.Id, stopienBieglosci.Nazwa, stopienBieglosci.Wartosc) : null;
    }
}