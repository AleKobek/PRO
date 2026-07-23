using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Profile.DTO.Profil;
using Squadra.Server.Modules.Profile.DTO.Status;
using Squadra.Server.Modules.Profile.Repositories;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Uzytkownicy.Repositories;

namespace Squadra.Server.Modules.Profile.Services;

public class ProfileService(
    IProfileRepository profileRepository, 
    IUzytkownicyRepository uzytkownicyRepository,
    IJezykiService jezykiService,
    IStatusyRepository statusyRepository) : IProfileService
{
    

    public async Task<ServiceResult<ProfilGetResDto>> GetProfil(int id)
    {
        try{
            if (id < 1) return ServiceResult<ProfilGetResDto>.BadRequest(new ErrorItem("Nieprawidłowe id profilu: " + id));
            
            var profil = await profileRepository.GetProfilUzytkownika(id);
            
            // podajemy status do wyświetlenia, czyli jeżeli użytkownik jest aktywny, to jego aktualny status, a jeżeli nie jest aktywny, to status offline
            var statusDoWyswietleniaRes = await GetStatusDoWyswietleniaProfilu(id);
            var statusDoWyswietlenia = statusDoWyswietleniaRes.Value;
            
            if(statusDoWyswietleniaRes.StatusCode == 404) return ServiceResult<ProfilGetResDto>.NotFound(statusDoWyswietleniaRes.Errors[0]);
            if(statusDoWyswietlenia == null) return ServiceResult<ProfilGetResDto>.NotFound(new ErrorItem("Status o podanym id nie istnieje")); // tylko aby nie było ostrzeżenia
            
            var profilDoZwrocenia = profil with {NazwaStatusu = statusDoWyswietlenia.Nazwa};
            return ServiceResult<ProfilGetResDto>.Ok(profilDoZwrocenia);
            
        }catch(NieZnalezionoWBazieException e){
            return ServiceResult<ProfilGetResDto>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<ProfilMinInfoDto>> GetProfilMinInfo(int id)
    {
        if(id < 1) return ServiceResult<ProfilMinInfoDto>.BadRequest(new ErrorItem("Nieprawidłowe id profilu: " + id));
        try
        { 
            var profil = await profileRepository.GetProfilMinInfo(id);
            var statusDoWyswietleniaRes = await GetStatusDoWyswietleniaProfilu(id);
            if(!statusDoWyswietleniaRes.Succeeded) return ServiceResult<ProfilMinInfoDto>.Fail(statusDoWyswietleniaRes.StatusCode, statusDoWyswietleniaRes.Errors);

            return ServiceResult<ProfilMinInfoDto>.Ok(profil with { NazwaStatusu = statusDoWyswietleniaRes.Value.Nazwa }); // .Value jest bezpieczne, bo jak się powiodło to nie jest null
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<ProfilMinInfoDto>.NotFound(new ErrorItem(e.Message));
        }
    }

    
    
    public async Task<ServiceResult<bool>> UpdateProfil(int id, ProfilUpdateDto profil)
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
        
        // tutaj zwracamy Not Found, aby nie mieszało się z resztą problemów walidacji
        if(id < 1) return ServiceResult<bool>.NotFound(new ErrorItem("Profil o id " + id + " nie istnieje"));
        if(profil.RegionId < 1) return ServiceResult<bool>.NotFound(new ErrorItem("Region o id " + profil.RegionId + " nie istnieje"));
        
        if(profil.Zaimki is { Length: > 30 })
        {
            bledy.Add(new ErrorItem("Maksymalna długość zaimków wynosi 30 znaków", nameof(profil.Zaimki)));
        }

        if (profil.Opis is { Length: > 300 })
        {
            bledy.Add(new ErrorItem("Maksymalna długość opisu wynosi 300 znaków", nameof(profil.Opis)));
        }

        if (profil.Pseudonim.Length > 20)
        {
            bledy.Add(new ErrorItem("Maksymalna długość pseudonimu wynosi 20 znaków", nameof(profil.Pseudonim)));
        }
        
        try
        {
            return bledy.Count > 0
                ? ServiceResult<bool>.BadRequest(bledy.ToArray())
                : ServiceResult<bool>.Ok(await profileRepository.UpdateProfil(id, profil), 204);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<bool>> UpdateAwatar(int id, IFormFile awatar)
    {
        if(awatar.Length == 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowy awatar"));
        try
        {
            var awatarWBajtach = await WspolneFunkcje.NormalizujObraz(awatar);
            return ServiceResult<bool>.Ok(await profileRepository.UpdateAwatar(id, awatarWBajtach), 204);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return  ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> DeleteProfil(int id)
    {
        if(id < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id profilu: " + id));
        try
        {
            // nie trzeba tworzyć transakcji, bo jest ona przy usuwaniu konta, a nie usuwamy profilu w innych sytuacjach
            var result = await jezykiService.ZmienJezykiProfilu(id, new List<JezykProfiluCreateDto>());
            if(result.StatusCode == 404) return ServiceResult<bool>.NotFound(result.Errors[0]);
            await profileRepository.DeleteProfil(id);
            return ServiceResult<bool>.Ok(true, 204);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<StatusDto>> GetStatusZBazyProfilu(int id)
    {
        try{
            return id < 1
                ? ServiceResult<StatusDto>.BadRequest(new ErrorItem("Profil o id " + id + " nie istnieje"))
                : ServiceResult<StatusDto>.Ok(await profileRepository.GetStatusProfilu(id));
        }catch(NieZnalezionoWBazieException e){
            return ServiceResult<StatusDto>.NotFound(new ErrorItem(e.Message));
        }
    }

    // zwracamy status do wyświetlenia, czyli jeżeli użytkownik jest aktywny, to jego aktualny status, a jeżeli nie jest aktywny, to status offline
    // zwracamy offline również, kiedy ma ustawiony status "niewidoczny"
    public async Task<ServiceResult<StatusDto>> GetStatusDoWyswietleniaProfilu(int id)
    {
        if(id < 1) return ServiceResult<StatusDto>.BadRequest(new ErrorItem("Nieprawidłowe id profilu: " + id));

        try
        {
            var status = await profileRepository.GetStatusProfilu(id);

            var aktywnosc = await uzytkownicyRepository.GetOstatniaAktywnoscUzytkownika(id);
            
            // uznajemy, że jest offline, jeżeli nie miał otwartego okna / połączenia przez 5 minut
            return ServiceResult<StatusDto>.Ok(DateTime.Now - aktywnosc > TimeSpan.FromMinutes(5) || status.Nazwa == "Niewidoczny" 
                ? statusyRepository.GetStatusOffline()
                : status);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<StatusDto>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<StatusDto>> UpdateStatus(int id, int idStatus)
    {
        try{
            if (id < 1)
                return ServiceResult<StatusDto>.BadRequest(new ErrorItem("Nieprawidłowe id profilu: " + id));

            if (idStatus < 1)
                return ServiceResult<StatusDto>.BadRequest(new ErrorItem("Nieprawidłowe id statusu: " + idStatus));

            return ServiceResult<StatusDto>.Ok(await profileRepository.UpdateStatus(id, idStatus));
            
        }catch(NieZnalezionoWBazieException e){
            return ServiceResult<StatusDto>.NotFound(new ErrorItem(e.Message));
        }
    }
}