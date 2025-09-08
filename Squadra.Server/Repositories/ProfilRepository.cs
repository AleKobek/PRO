using Microsoft.EntityFrameworkCore;

namespace Squadra;

public class ProfilRepository(AppDbContext appDbContext,
    IJezykRepository jezykRepository,
    IRegionRepository regionRepository)
{
    public async Task<ICollection<ProfilGetDto>> GetProfile()
    {
        ICollection<ProfilGetDto> profileDoZwrocenia = new List<ProfilGetDto>();
        ICollection<Profil> profile = await appDbContext.Profil.ToListAsync();

        foreach (var profil in profile)
        {
            var jezykiUzytkownika = await jezykRepository.GetJezykiUzytkownika(profil.IdUzytkownika);
            
            RegionDto? region = await regionRepository.GetRegion(profil.RegionId);
            profileDoZwrocenia.Add(new ProfilGetDto(
                profil.IdUzytkownika,
                profil.Pseudonim,
                region,
                profil.Zaimki,
                profil.Opis,
                jezykiUzytkownika,
                profil.Awatar
            ));
        }
        return profileDoZwrocenia;
    }

    public async Task<ProfilGetDto?> GetProfilUzytkownika(int id)
    {
        var profil = await appDbContext.Profil.FindAsync(id);
        
        if(profil == null) return null;
        
        var jezykiUzytkownika = await jezykRepository.GetJezykiUzytkownika(profil.IdUzytkownika);
        var region = await regionRepository.GetRegion(profil.RegionId);
        
        return new ProfilGetDto(profil.IdUzytkownika, profil.Pseudonim, region, profil.Zaimki, profil.Opis, jezykiUzytkownika, profil.Awatar);
    }
    
    
    //TODO dodać EF config dla profilu!
}