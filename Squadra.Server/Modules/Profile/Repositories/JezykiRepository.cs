using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Profile.Models;

namespace Squadra.Server.Modules.Profile.Repositories;

public class JezykiRepository(
    AppDbContext appDbContext,
    IStopnieBieglosciJezykaRepository stopnieBieglosciJezykaRepository)
    : IJezykiRepository
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
        ICollection<StopienBieglosciJezykaDto> stopnieBieglosci = await stopnieBieglosciJezykaRepository.GetStopnieBieglosciJezyka();
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
    
    // zastępujemy stare języki nowymi
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
    // funkcja zwracająca języki użytkownika ze stopniami o równej lub niższej wartości względem tej które on posiada dla danego języka
    public async Task<ICollection<JezykOrazRowneLubNizszeStopnieDto>> GetJezykiProfiluZeStopniamiRownymiLubNizszymi(int id)
    {
        ICollection<JezykOrazRowneLubNizszeStopnieDto> jezykiDoZwrocenia = new List<JezykOrazRowneLubNizszeStopnieDto>();
        ICollection<JezykProfilu> jezykiUzytkownika = await appDbContext.JezykProfilu.Where(x => x.UzytkownikId == id).ToListAsync();
        ICollection<JezykDto> jezyki = await GetJezyki();
        ICollection<StopienBieglosciJezykaDto> stopnieBieglosci = await stopnieBieglosciJezykaRepository.GetStopnieBieglosciJezyka();
        
        foreach (var var in jezykiUzytkownika)
        {
            var jezyk = jezyki.FirstOrDefault(x => x.Id == var.JezykId);
            var stopienBieglosci = stopnieBieglosci.FirstOrDefault(x => x.Id == var.StopienBieglosciId);
            if (jezyk != null && stopienBieglosci != null)
            {
                // Pobieramy wszystkie stopnie o równej lub niższej wartości względem posiadanego przez użytkownika
                var rowneLubNizszeStopnie = stopnieBieglosci.Where(s => s.Wartosc <= stopienBieglosci.Wartosc).ToList();
                jezykiDoZwrocenia.Add(new JezykOrazRowneLubNizszeStopnieDto(
                    new JezykDto(jezyk.Id, jezyk.Nazwa),
                    rowneLubNizszeStopnie
                ));
            }
        }
        
        return jezykiDoZwrocenia;
    }
}