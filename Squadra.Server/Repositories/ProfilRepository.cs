using Microsoft.EntityFrameworkCore;

namespace Squadra;

public class ProfilRepository(AppDbContext appDbContext,
    IJezykRepository jezykRepository,
    IRegionRepository regionRepository) : IProfilRepository
{
    public async Task<ICollection<ProfilGetDto>> GetProfile()
    {
        ICollection<ProfilGetDto> profileDoZwrocenia = new List<ProfilGetDto>();
        ICollection<Profil> profile = await appDbContext.Profil.ToListAsync();

        foreach (var profil in profile)
        {
            var jezykiUzytkownika = await jezykRepository.GetJezykiUzytkownika(profil.IdUzytkownika);
            
            var region = await regionRepository.GetRegion(profil.RegionId);
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

    public async Task<ProfilGetDto> GetProfilUzytkownika(int id)
    {
        var profil = await appDbContext.Profil.FindAsync(id);
        
        if(profil == null) throw new Exception("Profil o id " + id + " nie istnieje");
        
        var jezykiUzytkownika = await jezykRepository.GetJezykiUzytkownika(profil.IdUzytkownika);
        var region = await regionRepository.GetRegion(profil.RegionId);
        
        return new ProfilGetDto(profil.IdUzytkownika, profil.Pseudonim, region, profil.Zaimki, profil.Opis, jezykiUzytkownika, profil.Awatar);
    }

    public async Task<ProfilGetDto> UpdateProfil(ProfilUpdateDto profil)
    {
        var profilDoZmiany = await appDbContext.Profil.FindAsync(profil.IdUzytkownika);
        if(profilDoZmiany == null) throw new Exception("Profil uzytkownika o id " + profil.IdUzytkownika + " nie istnieje");
        
        profilDoZmiany.RegionId = profil.RegionId;
        profilDoZmiany.Zaimki = profil.Zaimki;
        profilDoZmiany.Opis = profil.Opis;
        profilDoZmiany.Awatar = profil.Awatar;

        await appDbContext.SaveChangesAsync();

        var jezykiUzytkownika = await jezykRepository.ZmienJezykiProfilu(profil.IdUzytkownika, profil.Jezyki);
        var region = await regionRepository.GetRegion(profil.RegionId);

        return new ProfilGetDto(
            profilDoZmiany.IdUzytkownika,
            profilDoZmiany.Pseudonim,
            region,
            profilDoZmiany.Zaimki,
            profilDoZmiany.Opis,
            jezykiUzytkownika,
            profilDoZmiany.Awatar
        );
    }

    public async Task<ProfilGetDto> CreateProfil(ProfilCreateDto profil)
    {
        var region = regionRepository.GetRegionDomyslny();
        var uzytkownik = await appDbContext.Uzytkownik.FindAsync(profil.IdUzytkownika);
        if(uzytkownik == null) throw new Exception("Uzytkownik o id " + profil.IdUzytkownika + " nie istnieje");
        var profilDoDodania = new Profil
        {
            IdUzytkownika = profil.IdUzytkownika,
            Awatar = [],
            JezykUzytkownikaCollection = new List<JezykProfilu>(),
            Opis = null,
            Pseudonim = profil.Pseudonim,
            Region = region,
            Zaimki = null,
            RegionId = region.Id,
            Uzytkownik = uzytkownik
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