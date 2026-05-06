using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Profile.Models;

namespace Squadra.Server.Modules.Profile.Repositories;

public class JezykRepository(
    AppDbContext appDbContext,
    IStopienBieglosciJezykaRepository stopienBieglosciJezykaRepository)
    : IJezykRepository
{
    public async Task<ICollection<Jezyk>> GetJezyki()
    {
        return await appDbContext.Jezyk.ToListAsync();
    }
    
    public async Task<Jezyk> GetJezyk(int id)
    {
        var jezyk = await appDbContext.Jezyk.FindAsync(id);
        if(jezyk == null ) throw new NieZnalezionoWBazieException("Nie znaleziono języka o id " + id);
        return jezyk;
    }

    public async Task<ICollection<JezykOrazStopienDto>> GetJezykiProfilu(int id)
    {
        ICollection<JezykOrazStopienDto> jezykiDoZwrocenia = new List<JezykOrazStopienDto>();
        ICollection<JezykProfilu> jezykiUzytkownika = await appDbContext.JezykProfilu.Where(x => x.UzytkownikId == id).ToListAsync();
        ICollection<Jezyk> jezyki = await GetJezyki();
        ICollection<StopienBieglosciJezyka> stopnieBieglosci = await stopienBieglosciJezykaRepository.GetStopnieBieglosciJezyka();
        foreach (var var in jezykiUzytkownika)
        {
            Jezyk? jezyk = jezyki.FirstOrDefault(x => x.Id == var.JezykId);
            StopienBieglosciJezyka? stopienBieglosci = stopnieBieglosci.FirstOrDefault(x => x.Id == var.StopienBieglosciId);
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
    
    public async Task<ICollection<JezykOrazStopienDto>> ZmienJezykiProfilu(int profilId, ICollection<JezykProfiluCreateDto> noweJezyki)
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
                JezykId = jezyk.JezykId,
                StopienBieglosciId = jezyk.StopienId
            });
        }

        await appDbContext.SaveChangesAsync();
        
        return await GetJezykiProfilu(profilId);
    }
    
}