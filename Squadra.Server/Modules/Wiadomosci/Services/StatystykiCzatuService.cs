using Squadra.Server.Exceptions;
using Squadra.Server.Repositories;

namespace Squadra.Server.Services;

// używamy tego w GetZnajomiDoListyUżytkownika w ZnajomiService
public class StatystykiCzatuService (IWiadomoscRepository wiadomoscRepository, IZnajomiRepository znajomiRepository) : IStatystykiCzatuService
{
    // potrzebujemy tego do sortowania znajomych na liście znajomych - ten, z którym mamy nowszą wiadomość, powinien być wyżej
    public async Task<ServiceResult<DateTime?>> GetDataNajnowszejWiadomosci(int idUzytkownika1, int idUzytkownika2)
    {
        try
        {
            // sprawdzamy, czy się wszystko zgadza
            if(idUzytkownika1 < 1) return ServiceResult<DateTime?>.NotFound(new ErrorItem("Uzytkownik o id " + idUzytkownika1 + " nie istnieje"));
            if(idUzytkownika2 < 1) return ServiceResult<DateTime?>.NotFound(new ErrorItem("Uzytkownik o id " + idUzytkownika2 + " nie istnieje"));
            if(idUzytkownika1 == idUzytkownika2) return ServiceResult<DateTime?>.BadRequest(new ErrorItem("Nie można pobrać wiadomości między tym samym użytkownikiem"));
            if(!await znajomiRepository.CzyJestZnajomosc(idUzytkownika1, idUzytkownika2))
                return ServiceResult<DateTime?>.BadRequest(new ErrorItem("Nie można pobrać wiadomości od użytkownika, który nie jest Twoim znajomym"));
            
            // jest git
            return ServiceResult<DateTime?>.Ok(await wiadomoscRepository.GetDataNajnowszejWiadomosci(idUzytkownika1, idUzytkownika2));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<DateTime?>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    // sprawdzamy, czy są nowe wiadomości od znajomego, czyli czy data najnowszej wiadomości jest większa niż data ostatniego otwarcia czatu z tym znajomym
    // potrzebujemy tego do wyróżnienia znajomego na liście, jeżeli są nowe wiadomości od niego
    public async Task<ServiceResult<bool>> CzySaNoweWiadomosciOdZnajomego(int idObecnegoUzytkownika, int idZnajomego)
    {
        try
        {
            if(idObecnegoUzytkownika < 1) return ServiceResult<bool>.NotFound(new ErrorItem("Uzytkownik o id " + idObecnegoUzytkownika + " nie istnieje"));
            if(idZnajomego < 1) return ServiceResult<bool>.NotFound(new ErrorItem("Uzytkownik o id " + idZnajomego + " nie istnieje"));
            
            if(idObecnegoUzytkownika == idZnajomego) return ServiceResult<bool>.BadRequest(new ErrorItem("Nie można sprawdzić wiadomości między tym samym użytkownikiem"));
            
            var dataNajnowszejWiadomosci = await wiadomoscRepository.GetDataNajnowszejWiadomosci(idObecnegoUzytkownika, idZnajomego);
            if(dataNajnowszejWiadomosci == null) return ServiceResult<bool>.Ok(false); // jeżeli nie ma żadnych wiadomości, to na pewno nie ma nowych
            
            // mamy już datę najnowszej wiadomości, teraz musimy sprawdzić, kiedy użytkownik ostatnio otworzył czat z tym znajomym
            try
            {
                var dataOstatniegoOtwarciaCzatu = await znajomiRepository.GetDataOstatniegoOtwarciaCzatu(idObecnegoUzytkownika, idZnajomego);
                return ServiceResult<bool>.Ok(dataNajnowszejWiadomosci > dataOstatniegoOtwarciaCzatu);
            }
            catch (NieZnalezionoWBazieException e)
            {
                return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
            }
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
}