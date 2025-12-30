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
            
            // trzeba tutaj oddzielnie sprawdzić, czy żaden użytkownik nie przekroczył maksymalnej liczby znajomych
            // bo powiadomienia mogą być wysyłane asynchronicznie i wtedy może się okazać, że obaj użytkownicy przekroczyli limit
            var znajomiUzytkownika1 = await znajomiRepository.GetZnajomiUzytkownika(idUzytkownika1);
            if (znajomiUzytkownika1.Count >= MaxLiczbaZnajomych)
                return ServiceResult<bool>.BadRequest(new ErrorItem("Uzytkownik o id " + idUzytkownika1 + " osiągnął maksymalną liczbę znajomych: " + MaxLiczbaZnajomych));
            var znajomiUzytkownika2 = await znajomiRepository.GetZnajomiUzytkownika(idUzytkownika2);
            if (znajomiUzytkownika2.Count >= MaxLiczbaZnajomych)
                return ServiceResult<bool>.BadRequest(new ErrorItem("Uzytkownik o id " + idUzytkownika2 + " osiągnął maksymalną liczbę znajomych: " + MaxLiczbaZnajomych));
            

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
            
            // historię wiadomości usuwa repozytorium, bo tam jest transakcja
            
            return ServiceResult<bool>.NoContent(await znajomiRepository.DeleteZnajomosc(idUzytkownikaInicjujacego, idUzytkownika2));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    // maksymalna liczba znajomych jednego użytkownika, statyczna wartość dostępna dla innych serwisów
    public const int MaxLiczbaZnajomych = 100;
}