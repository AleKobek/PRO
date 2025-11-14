using System.Text.RegularExpressions;
using Squadra.Server.DTO.Uzytkownik;
using Squadra.Server.Exceptions;
using Squadra.Server.Repositories;

namespace Squadra.Server.Services;

public class UzytkownikService(IUzytkownikRepository uzytkownikRepository) : IUzytkownikService
{
    public async Task<ServiceResult<ICollection<UzytkownikResDto>>> GetUzytkownicy()
    {
        return ServiceResult<ICollection<UzytkownikResDto>>.Ok(await uzytkownikRepository.GetUzytkownicy());
    }

    public async Task<ServiceResult<UzytkownikResDto>> GetUzytkownik(int id)
    {
        return id < 1 
            ? ServiceResult<UzytkownikResDto>.NotFound(new ErrorItem("Uzytkownik o id " + id + " nie istnieje")) 
            : ServiceResult<UzytkownikResDto>.Ok(await uzytkownikRepository.GetUzytkownik(id));
    }
    
    public async Task<ServiceResult<UzytkownikResDto>> CreateUzytkownik(UzytkownikCreateDto uzytkownik)
    {
        /*
         string Login,
         string Haslo,
         string Email,
         string? NumerTelefonu,
         DateOnly DataUrodzenia,
         string Pseudonim 
         */
        
        // sprawdzamy wszystkie dane, pseudonim niżej
        var bledy = await SprawdzPoprawnoscDanychPozaHaslem(
            // oznacza że nie ma tego użytkownika jeszcze
            -1,
            uzytkownik.Login,
            uzytkownik.Email,
            uzytkownik.NumerTelefonu,
            uzytkownik.DataUrodzenia
        );
        
        var bladHasla = SprawdzPoprawnoscHasla(uzytkownik.Haslo);
        if (bladHasla != null)
            bledy.Add(bladHasla);

        // sprawdzamy jeszcze pseudonim
        if (string.IsNullOrWhiteSpace(uzytkownik.Pseudonim) || uzytkownik.Pseudonim.Length > 20)
        {
            bledy.Add(new ErrorItem("Niepoprawny pseudonim.", nameof(uzytkownik.Pseudonim), "NiepoprawnyPseudonim"));
        }
        
        // jeżeli są jakieś błędy
        if (bledy.Count > 0)
        {
            // jeżeli którykolwiek z nich to błąd "coś już istnieje"
            var czySaKonflikty = bledy.Any(e => e.Code is "LoginIstnieje" or "EmailIstnieje");
            return czySaKonflikty
                ? ServiceResult<UzytkownikResDto>.Conflict(bledy.ToArray())
                : ServiceResult<UzytkownikResDto>.BadRequest(bledy.ToArray());
        }
        // skoro tu doszliśmy, wszystko jest git
        var stworzonyUzytkownik = await uzytkownikRepository.CreateUzytkownik(uzytkownik);
        
        return ServiceResult<UzytkownikResDto>.Ok(stworzonyUzytkownik, 201);

    }
    public async Task<ServiceResult<bool>> UpdateUzytkownik(int id, UzytkownikUpdateDto dto)
    {
        try
        {
            /*
             int Id,
             string Login,
             string Email,
             string? NumerTelefonu,
             DateOnly DataUrodzenia 
             */
            
            
            
            var bledy = await SprawdzPoprawnoscDanychPozaHaslem(
                id,
                dto.Login,
                dto.Email,
                dto.NumerTelefonu,
                dto.DataUrodzenia
            );
            
            if (bledy.Count > 0)
            {
                return ServiceResult<bool>.BadRequest(bledy.ToArray());
            }
            
            // jak tu doszliśmy, jedyne co może nie być git, jest to, że nie znajdzie. ale to już łapiemy
            var result = await uzytkownikRepository.UpdateUzytkownik(id, dto);
            return ServiceResult<bool>.Ok(result, 204);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<ICollection<string>>> UpdateHaslo(int idUzytkownika, string stareHaslo, string noweHaslo)
    {
        if(idUzytkownika < 1) return ServiceResult<ICollection<string>>.NotFound(new ErrorItem("Uzytkownik o id " + idUzytkownika + " nie istnieje"));
        if (stareHaslo == noweHaslo)
            return ServiceResult<ICollection<string>>.BadRequest(new ErrorItem("Stare hasło nie może być takie samo jak nowe hasło"));

        var bladHasla = SprawdzPoprawnoscHasla(noweHaslo);
        if (bladHasla != null) ServiceResult<ICollection<string>>.BadRequest(bladHasla);
        try
        {
            var wynikZBledami = await uzytkownikRepository.UpdateHaslo(idUzytkownika, stareHaslo, noweHaslo);
            // jeżeli jest git
            if (wynikZBledami.Count == 0) return ServiceResult<ICollection<string>>.Ok(wynikZBledami);
            // jeżeli nie jest git
            var bledy = wynikZBledami.Select(e => new ErrorItem(e, nameof(noweHaslo)));
            return ServiceResult<ICollection<string>>.BadRequest(bledy.ToArray());
        }catch(NieZnalezionoWBazieException e){
            return ServiceResult<ICollection<string>>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> DeleteUzytkownik(int id)
    {
        if(id < 1) return ServiceResult<bool>.NotFound(new ErrorItem("Uzytkownik o id " + id + " nie istnieje"));
        try
        {
            await uzytkownikRepository.DeleteUzytkownik(id);
        }catch(NieZnalezionoWBazieException e){
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
        return ServiceResult<bool>.Ok(true, 204);
    }
    private async Task<bool> CzyLoginIstnieje(int idUzytkownika, string login) => await uzytkownikRepository.CzyLoginIstnieje(idUzytkownika, login);
    private async Task<bool> CzyEmailIstnieje(int idUzytkownika, string email) => await uzytkownikRepository.CzyEmailIstnieje(idUzytkownika, email);

    // tutaj wyciągamy aby użyć też w update
    private async Task<List<ErrorItem>> SprawdzPoprawnoscDanychPozaHaslem(int id, string Login, string Email, string? NumerTelefonu, DateOnly DataUrodzenia)
    {
        var bledy = new List<ErrorItem>();
        
        if (await CzyLoginIstnieje(id, Login))
            bledy.Add(new ErrorItem("Taki login już istnieje.", nameof(Login), "LoginIstnieje"));

        if (await CzyEmailIstnieje(id, Email))
            bledy.Add(new ErrorItem("Taki email już istnieje.", nameof(Email), "EmailIstnieje"));
        
        var re = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        if (!re.IsMatch(Email))
        {
            bledy.Add(new ErrorItem("Niepoprawny email.", nameof(Email), "NiepoprawnyEmail"));       
        }
        
        re = new Regex("^([0-9]{9})$|^[0-9]{3}-[0-9]{3}-[0-9]{3}|^[0-9]{3} [0-9]{3} [0-9]{3}$");
        if (string.IsNullOrWhiteSpace(NumerTelefonu) || !re.IsMatch(NumerTelefonu))
        {
            bledy.Add(new ErrorItem("Niepoprawny numer telefonu.", nameof(NumerTelefonu), "NiepoprawnyNumerTelefonu"));
        }

        if (DataUrodzenia.Year < 1900 || DateTime.Now.Year - DataUrodzenia.Year < 18)
        {
            bledy.Add(new ErrorItem("Niepoprawna data urodzenia.", nameof(DataUrodzenia), "NiepoprawnaDataUrodzenia"));
        }
        return bledy;
    }
    private ErrorItem? SprawdzPoprawnoscHasla(string haslo)
    {
        var re = new Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*])(?=.{8,128})");
        if (!re.IsMatch(haslo))
            return new ErrorItem("Niepoprawne hasło.", nameof(haslo), "NiepoprawneHaslo");
        return null;
    }
}