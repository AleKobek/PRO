using Squadra.Server.DTO.Profil;
using Squadra.Server.Repositories;

namespace Squadra.Server.Services;

public class ProfilService(
    IProfilRepository profilRepository) : IProfilService
{

    public async Task<ProfilGetResDto> GetProfil(int id)
    {
        if(id < 1) throw new Exception("Profil o id " + id + " nie istnieje");

        return await profilRepository.GetProfilUzytkownika(id);
    }
    
    public async Task<ProfilUpdateResDto> UpdateProfil(int id, ProfilUpdateDto profil)
    {
        /*
            dane przyjdą w formie:
            int IdUzytkownika,
            int RegionId,
            string? Zaimki,
            string? Opis,
            ICollection<JezykOrazStopienDto> Jezyki,
            string Pseudonim,
            byte[]? Awatar
        */
        
        var czyPoprawne = true;
        var bladPseudonimu = "";
        var bladZaimkow = "";
        var bladOpisu = "";
        
        if(id < 1) throw new Exception("Profil o id " + id + " nie istnieje");
        if(profil.RegionId < 1) throw new Exception("Region o id " + profil.RegionId + " nie istnieje");
        
        if(profil.Zaimki is { Length: > 10 })
        {
            czyPoprawne = false;
            bladZaimkow = "Maksymalna długość zaimków wynosi 10 znaków";

        }

        if (profil.Opis is { Length: > 100 })
        {
            czyPoprawne = false;
            bladOpisu = "Maksymalna długość opisu wynosi 100 znaków";
        }

        if (profil.Pseudonim.Length > 20)
        {
            czyPoprawne = false;
            bladPseudonimu = "Maksymalna długość pseudonimu wynosi 20 znaków";
        }

        var profilDoZwrocenia = czyPoprawne ? await profilRepository.UpdateProfil(id, profil) : null;

        return new ProfilUpdateResDto(
            profilDoZwrocenia,
            new ProfilUpdateBledyDto(bladPseudonimu, bladZaimkow, bladOpisu),
            czyPoprawne);
    }
}