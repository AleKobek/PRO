using Microsoft.EntityFrameworkCore;
using Praca_Inzynierska.Context;
using Praca_Inzynierska.DTO;
using Praca_Inzynierska.Models;

namespace Praca_Inzynierska.Repositories;

public class JezykRepository : IJezykRepository
{
    private readonly AppDbContext _context;
    private readonly IStopienBieglosciJezykaRepository _stopienBieglosciJezykaRepository;

    public JezykRepository(AppDbContext appDbContext)
    {
        _context = appDbContext;
    }

    public async Task<ICollection<JezykDto>> GetJezyki()
    {
        ICollection<JezykDto> jezykiDoZwrocenia =  new List<JezykDto>();
        ICollection<Jezyk> jezyki = await _context.Jezyk.ToListAsync();
        foreach (var jezyk in jezyki)
        {
            jezykiDoZwrocenia.Add(new JezykDto(jezyk.Id, jezyk.Nazwa));
        }
        
        return jezykiDoZwrocenia;
    }

    public async Task<ICollection<JezykOrazStopienDto>> GetJezykiUzytkownika(int id)
    {
        ICollection<JezykOrazStopienDto> jezykiDoZwrocenia = new List<JezykOrazStopienDto>();
        ICollection<JezykUzytkownika> jezykiUzytkownika = await _context.JezykUzytkownika.Where(x => x.UzytkownikId == id).ToListAsync();
        ICollection<JezykDto> jezyki = await GetJezyki();
        ICollection<StopienBieglosciJezykaDto> stopnieBieglosci = await _stopienBieglosciJezykaRepository.GetStopnieBieglosciJezyka();
        foreach (var var in jezykiUzytkownika)
        {
            // bierzemy język który się zgadza z id
            JezykDto jezyk = jezyki.FirstOrDefault(x => x.Id == var.JezykId);
            StopienBieglosciJezykaDto stopienBieglosci = stopnieBieglosci.FirstOrDefault(x => x.Id == var.StopienBieglosciId);
            if(jezyk != null &&  stopienBieglosci != null)
            {
                jezykiDoZwrocenia.Add(new JezykOrazStopienDto(
                    jezyk,
                    stopienBieglosci
                ));
            }
        }

        return jezykiDoZwrocenia;
    }
    
}