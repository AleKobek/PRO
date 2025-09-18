using Microsoft.EntityFrameworkCore;
using Squadra.Exceptions;


namespace Squadra;

public class JezykRepository(
    AppDbContext appDbContext,
    IStopienBieglosciJezykaRepository stopienBieglosciJezykaRepository)
    : IJezykRepository
{
    public async Task<ICollection<JezykDto>> GetJezyki()
    {
        ICollection<JezykDto> jezykiDoZwrocenia =  new List<JezykDto>();
        ICollection<Jezyk> jezyki = await appDbContext.Jezyk.ToListAsync();
        foreach (var jezyk in jezyki)
        {
            jezykiDoZwrocenia.Add(new JezykDto(jezyk.Id, jezyk.Nazwa));
        }
        
        return jezykiDoZwrocenia;
    }
    
    public async Task<JezykDto?> GetJezyk(int id)
    {
        Jezyk? jezyk = await appDbContext.Jezyk.FindAsync(id);
        return jezyk != null ? new JezykDto(jezyk.Id, jezyk.Nazwa) : null;
    }

    public async Task<ICollection<JezykOrazStopienDto>> GetJezykiProfilu(int id)
    {
        ICollection<JezykOrazStopienDto> jezykiDoZwrocenia = new List<JezykOrazStopienDto>();
        ICollection<JezykProfilu> jezykiUzytkownika = await appDbContext.JezykProfilu.Where(x => x.UzytkownikId == id).ToListAsync();
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
    
    public async Task<ICollection<JezykOrazStopienDto>> ZmienJezykiProfilu(int profilId, ICollection<JezykOrazStopienDto> noweJezyki)
    {
        // sprawdzamy czy profil o id profilId istnieje
        var profil = await appDbContext.Profil.FindAsync(profilId);
        if(profil == null) throw new NieZnalezionoWBazieException("Profil o id " + profilId + " nie istnieje");
        
        // Usuwamy wszystkie istniejące powiązania języków dla tego profilu
        var jezykiDoUsuniecia = appDbContext.JezykProfilu.Where(jp => jp.UzytkownikId == profilId);
        appDbContext.JezykProfilu.RemoveRange(jezykiDoUsuniecia);

        // Dodajemy nowe powiązania języków
        foreach (var jezyk in noweJezyki)
        {
            appDbContext.JezykProfilu.Add(new JezykProfilu
            {
                UzytkownikId = profilId,
                JezykId = jezyk.Jezyk.Id,
                StopienBieglosciId = jezyk.Stopien.Id
            });
        }

        await appDbContext.SaveChangesAsync();
        
        return await GetJezykiProfilu(profilId);
    }
    
}