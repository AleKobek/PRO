using Microsoft.IdentityModel.Tokens;
using Squadra.Server.DTO.Wiadomosc;
using Squadra.Server.Exceptions;
using Squadra.Server.Repositories;

namespace Squadra.Server.Services;

public class WiadomoscService(IWiadomoscRepository wiadomoscRepository) : IWiadomoscService
{
    public async Task<ServiceResult<WiadomoscDto>> GetWiadomosc(int idWiadomosci, int idObecnegoUzytkownika)
    {
        try
        {
            if (idWiadomosci < 1) return ServiceResult<WiadomoscDto>.NotFound(new ErrorItem("Wiadomosc o id " + idWiadomosci + " nie istnieje"));
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
    public async Task<ServiceResult<ICollection<WiadomoscDto>>> GetWiadomosci(int idUzytkownika1, int idUzytkownika2)
    {
        try
        {
            if(idUzytkownika1 < 1) return ServiceResult<ICollection<WiadomoscDto>>.NotFound(new ErrorItem("Uzytkownik o id " + idUzytkownika1 + " nie istnieje"));
            if(idUzytkownika2 < 1) return ServiceResult<ICollection<WiadomoscDto>>.NotFound(new ErrorItem("Uzytkownik o id " + idUzytkownika2 + " nie istnieje"));
            if(idUzytkownika1 == idUzytkownika2) return ServiceResult<ICollection<WiadomoscDto>>.BadRequest(new ErrorItem("Nie można pobrać wiadomości między tym samym użytkownikiem"));
            
            return ServiceResult<ICollection<WiadomoscDto>>.Ok(await wiadomoscRepository.GetWiadomosci(idUzytkownika1, idUzytkownika2));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<ICollection<WiadomoscDto>>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> CreateWiadomosc(WiadomoscCreateDto wiadomosc)
    {
        try
        {
            if(wiadomosc.IdNadawcy == wiadomosc.IdOdbiorcy) 
                return ServiceResult<bool>.BadRequest(new ErrorItem("Nadawca i odbiorca wiadomości nie mogą być tym samym użytkownikiem"));
            
            if(wiadomosc.Tresc.IsNullOrEmpty())
                return ServiceResult<bool>.BadRequest(new ErrorItem("Treść wiadomości nie może być pusta"));
            
            if(wiadomosc.Tresc.Length > 1000)
                return ServiceResult<bool>.BadRequest(new ErrorItem("Treść wiadomości nie może przekraczać 1000 znaków"));
            
            return ServiceResult<bool>.Created(await wiadomoscRepository.CreateWiadomosc(wiadomosc));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> DeleteWiadomosciUzytkownikow(int idUzytkownika1, int idUzytkownika2)
    {
        try
        {
            if(idUzytkownika1 < 1) return ServiceResult<bool>.NotFound(new ErrorItem("Uzytkownik o id " + idUzytkownika1 + " nie istnieje"));
            if(idUzytkownika2 < 1) return ServiceResult<bool>.NotFound(new ErrorItem("Uzytkownik o id " + idUzytkownika2 + " nie istnieje"));
            if(idUzytkownika1 == idUzytkownika2) return ServiceResult<bool>.BadRequest(new ErrorItem("Nie można usunąć wiadomości między tym samym użytkownikiem"));
            return ServiceResult<bool>.Ok(await wiadomoscRepository.DeleteWiadomosciUzytkownikow(idUzytkownika1, idUzytkownika2));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
}