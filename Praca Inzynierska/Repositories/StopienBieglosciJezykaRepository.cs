using Microsoft.EntityFrameworkCore;
using Praca_Inzynierska.Context;
using Praca_Inzynierska.DTO;
using Praca_Inzynierska.Models;

namespace Praca_Inzynierska.Repositories;

public class StopienBieglosciJezykaRepository : IStopienBieglosciJezykaRepository
{
    private readonly AppDbContext _context;

    public StopienBieglosciJezykaRepository(AppDbContext appDbContext)
    {
        _context = appDbContext;
    }

    public async Task<ICollection<StopienBieglosciJezykaDto>> GetStopnieBieglosciJezyka()
    {
        ICollection<StopienBieglosciJezykaDto> stopnieBieglosciDoZwrocenia = new List<StopienBieglosciJezykaDto>();
        ICollection<StopienBieglosciJezyka> stopnieBieglosci = await _context.StopienBieglosciJezyka.ToListAsync();
        foreach (var stopienBieglosci in stopnieBieglosci)
        {
            stopnieBieglosciDoZwrocenia.Add(new StopienBieglosciJezykaDto(stopienBieglosci.Id, stopienBieglosci.Nazwa, stopienBieglosci.Wartosc));
        }

        return stopnieBieglosciDoZwrocenia;
    }

    public async Task<StopienBieglosciJezykaDto?> GetStopienBieglosciJezyka(int id)
    {
        StopienBieglosciJezyka? stopienBieglosci = await _context.StopienBieglosciJezyka.Where(x => x.Id == id).FirstOrDefaultAsync();
        return stopienBieglosci != null ? new StopienBieglosciJezykaDto(stopienBieglosci.Id, stopienBieglosci.Nazwa, stopienBieglosci.Wartosc) : null;
    }
}