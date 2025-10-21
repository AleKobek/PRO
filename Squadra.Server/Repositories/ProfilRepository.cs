using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.DTO.JezykStopien;
using Squadra.Server.DTO.Profil;
using Squadra.Server.DTO.Status;
using Squadra.Server.DTO.Uzytkownik;
using Squadra.Server.Exceptions;
using Squadra.Server.Models;

namespace Squadra.Server.Repositories;

public class ProfilRepository(AppDbContext appDbContext,
    IJezykRepository jezykRepository,
    IRegionRepository regionRepository,
    IStatusRepository statusRepository) : IProfilRepository
{
    public async Task<ICollection<ProfilGetResDto>> GetProfile()
    {
        ICollection<ProfilGetResDto> profileDoZwrocenia = new List<ProfilGetResDto>();
        ICollection<Profil> profile = await appDbContext.Profil.ToListAsync();

        foreach (var profil in profile)
        {
            var jezykiUzytkownika = await jezykRepository.GetJezykiProfilu(profil.IdUzytkownika);
            
            var regionIKraj = await regionRepository.GetRegionIKraj(profil.RegionId);
            var status = await statusRepository.GetStatus(profil.StatusId) ?? statusRepository.GetStatusDomyslny();
            profileDoZwrocenia.Add(new ProfilGetResDto(
                profil.IdUzytkownika,
                profil.Pseudonim,
                regionIKraj,
                profil.Zaimki,
                profil.Opis,
                jezykiUzytkownika,
                profil.Awatar,
                status
            ));
        }
        return profileDoZwrocenia;
    }

    public async Task<ProfilGetResDto> GetProfilUzytkownika(int id)
    {
        var profil = await appDbContext.Profil.FindAsync(id);
        
        if(profil == null) throw new NieZnalezionoWBazieException("Profil o id " + id + " nie istnieje");
        
        // Dane mają zostać wysłane w formie: {pseudonim, zaimki, kraj {id, nazwa} , region {id, nazwa}, opis}
        
        var jezykiUzytkownika = await jezykRepository.GetJezykiProfilu(profil.IdUzytkownika);
        
        var region = profil.RegionId == null ? null : await regionRepository.GetRegionIKraj(profil.RegionId);
        var status = await statusRepository.GetStatus(profil.StatusId) ?? statusRepository.GetStatusDomyslny();
        
        return new ProfilGetResDto(profil.IdUzytkownika, profil.Pseudonim, region, profil.Zaimki, profil.Opis, jezykiUzytkownika, profil.Awatar, status);
    }

    public async Task<ProfilGetResDto> UpdateProfil(int id, ProfilUpdateDto profil)
    {
        var profilDoZmiany = await appDbContext.Profil.FindAsync(id);
        if(profilDoZmiany == null) throw new NieZnalezionoWBazieException("Profil uzytkownika o id " + id + " nie istnieje");
        
        profilDoZmiany.Pseudonim = profil.Pseudonim;
        profilDoZmiany.RegionId = profil.RegionId;
        profilDoZmiany.Zaimki = profil.Zaimki;
        profilDoZmiany.Opis = profil.Opis;
        profilDoZmiany.Awatar = profil.Awatar;

        await appDbContext.SaveChangesAsync();

        var jezykiUzytkownika = await jezykRepository.ZmienJezykiProfilu(id, profil.Jezyki);
        var region = profil.RegionId == null ? null : await regionRepository.GetRegionIKraj(profil.RegionId);
        var status = await statusRepository.GetStatus(profilDoZmiany.StatusId) ?? statusRepository.GetStatusDomyslny();

        return new ProfilGetResDto(
            profilDoZmiany.IdUzytkownika,
            profilDoZmiany.Pseudonim,
            region,
            profilDoZmiany.Zaimki,
            profilDoZmiany.Opis,
            jezykiUzytkownika,
            profilDoZmiany.Awatar,
            status
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
        if(profil == null) throw new NieZnalezionoWBazieException("Profil o id " + id + " nie istnieje");
        // usuwamy wszystkie języki, podajemy pustą listę
        await jezykRepository.ZmienJezykiProfilu(id, new List<JezykOrazStopienDto>());
        appDbContext.Profil.Remove(profil);
        await appDbContext.SaveChangesAsync();
    }
    
    public async Task<StatusDto> GetStatusProfilu(int id)
    {
        var profil = await appDbContext.Profil.FindAsync(id);
        if(profil == null) throw new Exception("Profil o id uzytkownika " + id + " nie istnieje");
        
        var status = await statusRepository.GetStatus(profil.StatusId) ?? statusRepository.GetStatusDomyslny();
        return status;
    }
    
    
    public async Task<ProfilGetResDto> UpdateStatus(int id, int idStatus)
    {
        var profil = await appDbContext.Profil.FindAsync(id);
        if(profil == null) throw new Exception("Profil o id uzytkownika " + id + " nie istnieje");
        profil.StatusId = idStatus;
        await appDbContext.SaveChangesAsync();
        return await GetProfilUzytkownika(id);
    }
    
}