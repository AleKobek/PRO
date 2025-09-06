using Microsoft.EntityFrameworkCore;


namespace Squadra;

public class JezykRepository(
    AppDbContext context,
    IStopienBieglosciJezykaRepository stopienBieglosciJezykaRepository)
    : IJezykRepository
{
    public async Task<ICollection<JezykDto>> GetJezyki()
    {
        ICollection<JezykDto> jezykiDoZwrocenia =  new List<JezykDto>();
        ICollection<Jezyk> jezyki = await context.Jezyk.ToListAsync();
        foreach (var jezyk in jezyki)
        {
            jezykiDoZwrocenia.Add(new JezykDto(jezyk.Id, jezyk.Nazwa));
        }
        
        return jezykiDoZwrocenia;
    }
    
    public async Task<JezykDto?> GetJezyk(int id)
    {
        Jezyk? jezyk = await context.Jezyk.FindAsync(id);
        return jezyk != null ? new JezykDto(jezyk.Id, jezyk.Nazwa) : null;
    }

    public async Task<ICollection<JezykOrazStopienDto>> GetJezykiUzytkownika(int id)
    {
        ICollection<JezykOrazStopienDto> jezykiDoZwrocenia = new List<JezykOrazStopienDto>();
        ICollection<JezykUzytkownika> jezykiUzytkownika = await context.JezykUzytkownika.Where(x => x.UzytkownikId == id).ToListAsync();
        ICollection<JezykDto> jezyki = await GetJezyki();
        ICollection<StopienBieglosciJezykaDto> stopnieBieglosci = await stopienBieglosciJezykaRepository.GetStopnieBieglosciJezyka();
        foreach (var var in jezykiUzytkownika)
        {
            JezykDto? jezyk = jezyki.FirstOrDefault(x => x.Id == var.JezykId);
            StopienBieglosciJezykaDto? stopienBieglosci = stopnieBieglosci.FirstOrDefault(x => x.Id == var.StopienBieglosciId);
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