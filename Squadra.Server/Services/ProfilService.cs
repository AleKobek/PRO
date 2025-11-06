using Squadra.Server.DTO.Profil;
using Squadra.Server.DTO.Status;
using Squadra.Server.Exceptions;
using Squadra.Server.Repositories;

namespace Squadra.Server.Services;

public class ProfilService(
    IProfilRepository profilRepository) : IProfilService
{

    public async Task<ServiceResult<ProfilGetResDto>> GetProfil(int id)
    {
        try{
            return id < 1
                ? ServiceResult<ProfilGetResDto>.NotFound(new ErrorItem("Profil o id " + id + " nie istnieje"))
                : ServiceResult<ProfilGetResDto>.Ok(await profilRepository.GetProfilUzytkownika(id));
        }catch(NieZnalezionoWBazieException e){
            return ServiceResult<ProfilGetResDto>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    
    public async Task<ServiceResult<ProfilGetResDto>> UpdateProfil(int id, ProfilUpdateDto profil)
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
        
        var bledy = new List<ErrorItem>();
        
        if(id < 1) return ServiceResult<ProfilGetResDto>.BadRequest(new ErrorItem("Profil o id " + id + " nie istnieje"));
        if(profil.RegionId < 1) return ServiceResult<ProfilGetResDto>.BadRequest(new ErrorItem("Region o id " + profil.RegionId + " nie istnieje"));
        
        if(profil.Zaimki is { Length: > 10 })
        {
            bledy.Add(new ErrorItem("Maksymalna długość zaimków wynosi 10 znaków", nameof(profil.Zaimki)));
        }

        if (profil.Opis is { Length: > 100 })
        {
            bledy.Add(new ErrorItem("Maksymalna długość opisu wynosi 100 znaków", nameof(profil.Opis)));
        }

        if (profil.Pseudonim.Length > 20)
        {
            bledy.Add(new ErrorItem("Maksymalna długość pseudonimu wynosi 20 znaków", nameof(profil.Pseudonim)));
        }

        try
        {
            return bledy.Count > 0
                ? ServiceResult<ProfilGetResDto>.BadRequest(bledy.ToArray())
                : ServiceResult<ProfilGetResDto>.Ok(await profilRepository.UpdateProfil(id, profil));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<ProfilGetResDto>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<StatusDto>> GetStatusZBazyProfilu(int id)
    {
        try{
            return id < 1
                ? ServiceResult<StatusDto>.BadRequest(new ErrorItem("Profil o id " + id + " nie istnieje"))
                : ServiceResult<StatusDto>.Ok(await profilRepository.GetStatusProfilu(id));
        }catch(NieZnalezionoWBazieException e){
            return ServiceResult<StatusDto>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<StatusDto>> GetStatusDoWyswietleniaProfilu(int id)
    {
        if(id < 1) return ServiceResult<StatusDto>.NotFound(new ErrorItem("Profil o id " + id + " nie istnieje"));

        try
        {
            var status = await profilRepository.GetStatusProfilu(id);
            
            // jeżeli jest offline, zawsze wyświetlamy offline (nie mamy jeszcze jak tego sprawdzić)
            // zakładamy na razie, że cały czas jest online
            return ServiceResult<StatusDto>.Ok(status.Nazwa == "Niewidoczny"? new StatusDto(5, "Offline") : status);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<StatusDto>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<ProfilGetResDto>> UpdateStatus(int id, int idStatus)
    {
        try{
            if (id < 1)
                return ServiceResult<ProfilGetResDto>.BadRequest(new ErrorItem("Profil o id " + id + " nie istnieje"));

            if (idStatus < 1)
                return ServiceResult<ProfilGetResDto>.BadRequest(new ErrorItem("Status o id " + id + " nie istnieje"));

            return ServiceResult<ProfilGetResDto>.Ok(await profilRepository.UpdateStatus(id, idStatus));
            
        }catch(NieZnalezionoWBazieException e){
            return ServiceResult<ProfilGetResDto>.NotFound(new ErrorItem(e.Message));
        }
    }
}