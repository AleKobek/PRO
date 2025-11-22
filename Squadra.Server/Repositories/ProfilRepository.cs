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
            var status = await statusRepository.GetStatus(profil.StatusId) ?? statusRepository.GetStatusOffline();

            profileDoZwrocenia.Add(new ProfilGetResDto(
                profil.Pseudonim,
                regionIKraj,
                profil.Zaimki,
                profil.Opis,
                jezykiUzytkownika,
                profil.Awatar,
                status.Nazwa
            ));
        }
        return profileDoZwrocenia;
    }

    public async Task<ProfilGetResDto> GetProfilUzytkownika(int id)
    {
        var profil = await appDbContext.Profil.FindAsync(id);
        
        if(profil == null) throw new NieZnalezionoWBazieException("Profil o id " + id + " nie istnieje");
        
        // Dane mają zostać wysłane w formie: {pseudonim, zaimki, kraj {id, nazwa} , region {id, nazwa}, opis, awatar, nazwa statusu}
        
        var jezykiUzytkownika = await jezykRepository.GetJezykiProfilu(profil.IdUzytkownika);
        
        var region = profil.RegionId == null ? null : await regionRepository.GetRegionIKraj(profil.RegionId);
        
        var status = await statusRepository.GetStatus(profil.StatusId) ?? statusRepository.GetStatusOffline();

        
        return new ProfilGetResDto(profil.Pseudonim, region, profil.Zaimki, profil.Opis, jezykiUzytkownika, profil.Awatar, status.Nazwa);
    }

    // bez awatara!
    public async Task<bool> UpdateProfil(int id, ProfilUpdateDto profil)
    {
        var profilDoZmiany = await appDbContext.Profil.FindAsync(id);
        if(profilDoZmiany == null) throw new NieZnalezionoWBazieException("Profil uzytkownika o id " + id + " nie istnieje");
        
        profilDoZmiany.Pseudonim = profil.Pseudonim;
        profilDoZmiany.RegionId = profil.RegionId;
        profilDoZmiany.Zaimki = profil.Zaimki;
        profilDoZmiany.Opis = profil.Opis;

        await appDbContext.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<bool> UpdateAwatar(int id, byte[] awatar)
    {
        var profilDoZmiany = await appDbContext.Profil.FindAsync(id);
        if(profilDoZmiany == null) throw new NieZnalezionoWBazieException("Profil uzytkownika o id " + id + " nie istnieje");

        if (awatar.Length <= 0) return true;
        profilDoZmiany.Awatar = awatar;

        await appDbContext.SaveChangesAsync();

        return true;
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
            RegionId = null,
            StatusId = 1
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
        await jezykRepository.ZmienJezykiProfilu(id, new List<JezykProfiluCreateDto>());
        appDbContext.Profil.Remove(profil);
        await appDbContext.SaveChangesAsync();
    }
    
    public async Task<StatusDto> GetStatusProfilu(int id)
    {
        var profil = await appDbContext.Profil.FindAsync(id);
        if(profil == null) throw new Exception("Profil o id uzytkownika " + id + " nie istnieje");
        
        var status = await statusRepository.GetStatus(profil.StatusId) ?? statusRepository.GetStatusOffline();
        return status;
    }
    
    
    // zwracamy status dto bo możemy od razu zmienić to w nagłówku bez pobierania na nowo
    public async Task<StatusDto> UpdateStatus(int id, int idStatus)
    {
        var profil = await appDbContext.Profil.FindAsync(id);
        if(profil == null) throw new NieZnalezionoWBazieException("Profil o id uzytkownika " + id + " nie istnieje");
        var status = await statusRepository.GetStatus(idStatus);
        if(status == null) throw new NieZnalezionoWBazieException("Status o id " + idStatus + " nie istnieje");
        profil.StatusId = idStatus;
        await appDbContext.SaveChangesAsync();
        return status;
    }
    
}