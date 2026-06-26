using Microsoft.IdentityModel.Tokens;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Drużyny.DTO;
using Squadra.Server.Modules.Drużyny.Services;
using Squadra.Server.Modules.Profile.DTO.Profil;
using Squadra.Server.Modules.Profile.Services;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Wiadomosci.DTO;
using Squadra.Server.Modules.Wiadomosci.Repositories;
using Squadra.Server.Modules.Znajomosci.Repositories;

namespace Squadra.Server.Modules.Wiadomosci.Services;

public class WiadomoscService(IWiadomoscRepository wiadomoscRepository,
    IZnajomiRepository znajomiRepository,
    IDruzynyService druzynyService,
    IProfilService profilService) : IWiadomoscService
{
    public async Task<ServiceResult<WiadomoscDto>> GetWiadomosc(int idWiadomosci, int idObecnegoUzytkownika)
    {
        try
        {
            if (idWiadomosci < 1) return ServiceResult<WiadomoscDto>.BadRequest(new ErrorItem("Nieprawidłowe id wiadomości: " + idWiadomosci));
            var wiadomosc = await wiadomoscRepository.GetWiadomosc(idWiadomosci);
            if (wiadomosc.IdNadawcy != idObecnegoUzytkownika && wiadomosc.IdOdbiorcy != idObecnegoUzytkownika)
                return ServiceResult<WiadomoscDto>.Forbidden(new ErrorItem("Brak dostępu do wiadomości o id " + idWiadomosci));
            return ServiceResult<WiadomoscDto>.Ok(wiadomosc);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<WiadomoscDto>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    // w kontrolerze sprawdzamy, czy idUzytkownika1 lub idUzytkownika2 to id obecnego użytkownika
    public async Task<ServiceResult<ICollection<WiadomoscDto>>> GetWiadomosciPrywatne(int idUzytkownika1, int idUzytkownika2)
    {
        try
        {
            if(idUzytkownika1 < 1) return ServiceResult<ICollection<WiadomoscDto>>.BadRequest(new ErrorItem("Nieprawidłowe id użytkownika: " + idUzytkownika1));
            if(idUzytkownika2 < 1) return ServiceResult<ICollection<WiadomoscDto>>.BadRequest(new ErrorItem("Nieprawidłowe id użytkownika: " + idUzytkownika2));
            if(idUzytkownika1 == idUzytkownika2) return ServiceResult<ICollection<WiadomoscDto>>.BadRequest(new ErrorItem("Nie można pobrać wiadomości między tym samym użytkownikiem"));
            
            return ServiceResult<ICollection<WiadomoscDto>>.Ok(await wiadomoscRepository.GetWiadomosciPrywatne(idUzytkownika1, idUzytkownika2));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<ICollection<WiadomoscDto>>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<CzatDruzynowyDto>> GetWiadomosciNaCzacieDruzyny(int idDruzyny, int idCzytajacego)
    {
        if(idDruzyny < 1) return ServiceResult<CzatDruzynowyDto>.BadRequest(new ErrorItem("Nieprawidłowe id drużyny: " + idDruzyny));
        
        // sprawdzamy, czy użytkownik należy do drużyny, bo tylko członkowie drużyny mogą czytać czat drużyny
        var czyUzytkownikNalezyDoDruzynyRes = await druzynyService.CzyUzytkownikNalezyDoDruzyny(idCzytajacego, idDruzyny);
        if(!czyUzytkownikNalezyDoDruzynyRes.Succeeded) return ServiceResult<CzatDruzynowyDto>.Fail(czyUzytkownikNalezyDoDruzynyRes.StatusCode, czyUzytkownikNalezyDoDruzynyRes.Errors);

        if(!czyUzytkownikNalezyDoDruzynyRes.Value)
            return ServiceResult<CzatDruzynowyDto>.Forbidden(new ErrorItem("Brak dostępu do czatu drużyny o id " + idDruzyny));
        
        // bierzemy wiadomości z czatu drużyny
        var wiadomosci = await wiadomoscRepository.GetWiadomosciNaCzacieDruzyny(idDruzyny);
        
        // bierzemy osoby, które brały udział w czacie
        var idNadawcow = wiadomosci.Select(x => x.IdNadawcy).Distinct().ToList();
        var nadawcy = new List<ProfilMinInfoDto>();
        foreach(var id in idNadawcow)
        {
            var profilRes = await profilService.GetProfilMinInfo(id);
            if(profilRes.Succeeded && profilRes.Value != null) 
                nadawcy.Add(profilRes.Value);
            // jeżeli nie znaleziono tego użytkownika, bo usunął konto, to nie dodajemy i na froncie będziemy wyświetlać "Nieznany użytkownik" lub coś takiego
        }
        
        return ServiceResult<CzatDruzynowyDto>.Ok(new CzatDruzynowyDto(nadawcy, wiadomosci));
    }

    
    public async Task<ServiceResult<bool>> CreateWiadomosc(int idOdbiorcy, WiadomoscCreateDto wiadomosc, int idObecnegoUzytkownika)
    {
        try
        {
            if(idObecnegoUzytkownika == idOdbiorcy) 
                return ServiceResult<bool>.BadRequest(new ErrorItem("Nadawca i odbiorca wiadomości nie mogą być tym samym użytkownikiem"));
            
            if(wiadomosc.Tresc.IsNullOrEmpty())
                return ServiceResult<bool>.BadRequest(new ErrorItem("Treść wiadomości nie może być pusta"));
            
            if(wiadomosc.Tresc.Length > 1000)   
                return ServiceResult<bool>.BadRequest(new ErrorItem("Treść wiadomości nie może przekraczać 1000 znaków"));
            
            if(!await znajomiRepository.CzyJestZnajomosc(idObecnegoUzytkownika, idOdbiorcy))
                return ServiceResult<bool>.BadRequest(new ErrorItem("Nie można wysłać wiadomości do użytkownika, który nie jest Twoim znajomym"));
            
            return ServiceResult<bool>.Created(await wiadomoscRepository.CreateWiadomosc(idOdbiorcy, wiadomosc, idObecnegoUzytkownika));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> DeleteWiadomosciPrywatneUzytkownikow(int idUzytkownika1, int idUzytkownika2)
    {
        try
        {
            if(idUzytkownika1 < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id użytkownika: " + idUzytkownika1));
            if(idUzytkownika2 < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id użytkownika: " + idUzytkownika2));
            if(idUzytkownika1 == idUzytkownika2) return ServiceResult<bool>.BadRequest(new ErrorItem("Nie można usunąć wiadomości między tym samym użytkownikiem"));
            return ServiceResult<bool>.Ok(await wiadomoscRepository.DeleteWiadomosciPrywatneUzytkownikow(idUzytkownika1, idUzytkownika2));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
}