using Microsoft.EntityFrameworkCore;

namespace Squadra;

public class ProfilRepository(AppDbContext appDbContext,
    IJezykRepository jezykRepository,
    IRegionRepository regionRepository) : IProfilRepository
{
    public async Task<ICollection<ProfilGetResDto>> GetProfile()
    {
        ICollection<ProfilGetResDto> profileDoZwrocenia = new List<ProfilGetResDto>();
        ICollection<Profil> profile = await appDbContext.Profil.ToListAsync();

        foreach (var profil in profile)
        {
            var jezykiUzytkownika = await jezykRepository.GetJezykiProfilu(profil.IdUzytkownika);
            
            var region = await regionRepository.GetRegionIKraj(profil.RegionId);
            profileDoZwrocenia.Add(new ProfilGetResDto(
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

    public async Task<ProfilGetResDto> GetProfilUzytkownika(int id)
    {
        var profil = await appDbContext.Profil.FindAsync(id);
        
        if(profil == null) throw new Exception("Profil o id " + id + " nie istnieje");
        
        // Dane mają zostać wysłane w formie: {pseudonim, zaimki, kraj {id, nazwa} , region {id, nazwa}, opis}
        
        var jezykiUzytkownika = await jezykRepository.GetJezykiProfilu(profil.IdUzytkownika);
        var region = await regionRepository.GetRegionIKraj(profil.RegionId);
        
        return new ProfilGetResDto(profil.IdUzytkownika, profil.Pseudonim, region, profil.Zaimki, profil.Opis, jezykiUzytkownika, profil.Awatar);
    }

    public async Task<ProfilGetResDto> UpdateProfil(ProfilUpdateDto profil)
    {
        var profilDoZmiany = await appDbContext.Profil.FindAsync(profil.IdUzytkownika);
        if(profilDoZmiany == null) throw new Exception("Profil uzytkownika o id " + profil.IdUzytkownika + " nie istnieje");
        
        profilDoZmiany.Pseudonim = profil.Pseudonim;
        profilDoZmiany.RegionId = profil.RegionId;
        profilDoZmiany.Zaimki = profil.Zaimki;
        profilDoZmiany.Opis = profil.Opis;
        profilDoZmiany.Awatar = profil.Awatar;

        await appDbContext.SaveChangesAsync();

        var jezykiUzytkownika = await jezykRepository.ZmienJezykiProfilu(profil.IdUzytkownika, profil.Jezyki);
        var region = profil.RegionId == null ? null : await regionRepository.GetRegionIKraj(profil.RegionId);

        return new ProfilGetResDto(
            profilDoZmiany.IdUzytkownika,
            profilDoZmiany.Pseudonim,
            region,
            profilDoZmiany.Zaimki,
            profilDoZmiany.Opis,
            jezykiUzytkownika,
            profilDoZmiany.Awatar
        );
    }

    public async Task<ProfilGetResDto> CreateProfil(ProfilCreateReqDto profil)
    {
        var profilDoDodania = new Profil
        {
            IdUzytkownika = profil.IdUzytkownika,
            Awatar = [],
            Opis = null,
            Pseudonim = profil.Pseudonim,
            Zaimki = null,
            RegionId = null
        };
        await appDbContext.Profil.AddAsync(profilDoDodania);
        await appDbContext.SaveChangesAsync();
        return await GetProfilUzytkownika(profil.IdUzytkownika);
    }

    public async Task DeleteProfil(int id)
    {
        var profil = await appDbContext.Profil.FindAsync(id);
        if(profil == null) throw new Exception("Profil o id " + id + " nie istnieje");
        // usuwamy wszystkie języki, podajemy pustą listę
        await jezykRepository.ZmienJezykiProfilu(id, new List<JezykOrazStopienDto>());
        appDbContext.Profil.Remove(profil);
        await appDbContext.SaveChangesAsync();
    }
    
}