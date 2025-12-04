using Squadra.Server.DTO.Profil;
using Squadra.Server.Exceptions;
using Squadra.Server.Repositories;

namespace Squadra.Server.Services;

public class ZnajomiService(IZnajomiRepository znajomiRepository) : IZnajomiService
{

    public async Task<ServiceResult<ICollection<ProfilGetResDto>>> GetZnajomiUzytkownika(int id)
    {
        if(id < 1) return ServiceResult<ICollection<ProfilGetResDto>>.NotFound(new ErrorItem("Użytkownik o id " + id + " nie istnieje"));
        
        try
        {
            
            return ServiceResult<ICollection<ProfilGetResDto>>.Ok(await znajomiRepository.GetZnajomiUzytkownika(id));
            
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<ICollection<ProfilGetResDto>>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<bool>> CreateZnajomosc(int idUzytkownika1, int idUzytkownika2)
    {
        try
        {
            if (idUzytkownika1 < 1) return ServiceResult<bool>.NotFound(new ErrorItem("Uzytkownik o id " + idUzytkownika1 + " nie istnieje"));
            if (idUzytkownika2 < 1) return ServiceResult<bool>.NotFound(new ErrorItem("Uzytkownik o id " + idUzytkownika2 + " nie istnieje"));

            return ServiceResult<bool>.Created(await znajomiRepository.CreateZnajomosc(idUzytkownika1, idUzytkownika2));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<bool>> DeleteZnajomosc(int idUzytkownikaInicjujacego, int idUzytkownika2)
    {
        try
        {
            if (idUzytkownikaInicjujacego < 1)
                return ServiceResult<bool>.NotFound(
                    new ErrorItem("Uzytkownik o id " + idUzytkownikaInicjujacego + " nie istnieje"));
            if (idUzytkownika2 < 1)
                return ServiceResult<bool>.NotFound(
                    new ErrorItem("Uzytkownik o id " + idUzytkownika2 + " nie istnieje"));
            
            //TODO wiadomościService.UsunHistorieWiadomosci(idUzytkownikaInicjujacego, idUzytkownika2)
            
            //TODO to niżej przenosimy do kontrolera, aby nie było w kółko
            
            // await powiadomienieService.CreatePowiadomienie(new PowiadomienieCreateDto(
            //     5,
            //     idUzytkownikaInicjujacego,
            //     idUzytkownika2,
            //     null
            // ));
            return ServiceResult<bool>.NoContent(await znajomiRepository.DeleteZnajomosc(idUzytkownikaInicjujacego, idUzytkownika2));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
}