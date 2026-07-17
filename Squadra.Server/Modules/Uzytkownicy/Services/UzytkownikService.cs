using System.Text.RegularExpressions;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Profile.Services;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Uzytkownicy.DTO.Uzytkownik;
using Squadra.Server.Modules.Uzytkownicy.Repositories;

namespace Squadra.Server.Modules.Uzytkownicy.Services;

public class UzytkownikService(
    IUzytkownikRepository uzytkownikRepository,
    IProfilService profilService) : IUzytkownikService
{
   

    public async Task<ServiceResult<UzytkownikResDto>> GetUzytkownik(int id)
    {
        try
        {
            return id < 1
                ? ServiceResult<UzytkownikResDto>.BadRequest(new ErrorItem("Niepoprawne id użytkownika: " + id))
                : ServiceResult<UzytkownikResDto>.Ok(await uzytkownikRepository.GetUzytkownik(id));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<UzytkownikResDto>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<UzytkownikResDto>> GetUzytkownik(string login)
    {
        try
        {
            return string.IsNullOrWhiteSpace(login)
                ? ServiceResult<UzytkownikResDto>.BadRequest(new ErrorItem("Login nie może być pusty."))
                : ServiceResult<UzytkownikResDto>.Ok(await uzytkownikRepository.GetUzytkownik(login));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<UzytkownikResDto>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<DateTime?>> GetOstatniaAktywnoscUzytkownika(int id)
    {
        if(id < 1) return ServiceResult<DateTime?>.BadRequest(new ErrorItem("Niepoprawne id użytkownika: " + id));
        try
        {
            var ostatniaAktywnosc = await uzytkownikRepository.GetOstatniaAktywnoscUzytkownika(id);
            return ServiceResult<DateTime?>.Ok(ostatniaAktywnosc);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<DateTime?>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<bool>> CzyUzytkownikJestAdminem(int id)
    {
        if(id < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Niepoprawne id użytkownika: " + id));
        try
        {
            return ServiceResult<bool>.Ok(await uzytkownikRepository.CzyUzytkownikJestAdminem(id));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<bool>> CzyUzytkownikMaZintegrowaneKonto(int id)
    {
        if(id < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Niepoprawne id użytkownika: " + id));
        try
        {
            var maZintegrowaneKonto = await uzytkownikRepository.CzyUzytkownikMaZintegrowaneKonto(id);
            return ServiceResult<bool>.Ok(maZintegrowaneKonto);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> CreateUzytkownik(UzytkownikCreateDto uzytkownik)
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
            null,
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
                ? ServiceResult<bool>.Conflict(bledy.ToArray())
                : ServiceResult<bool>.BadRequest(bledy.ToArray());
        }
        // skoro tu doszliśmy, wszystko jest w porządku
        return ServiceResult<bool>.Created(await uzytkownikRepository.CreateUzytkownik(uzytkownik));

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
            
            // jak tu doszliśmy, jedyne co może nie być w porządku, jest to, że nie znajdzie. ale to już łapiemy
            var result = await uzytkownikRepository.UpdateUzytkownik(id, dto);
            return ServiceResult<bool>.Ok(result, 204);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<bool>> UpdateHaslo(int idUzytkownika, string stareHaslo, string noweHaslo)
    {
        if(idUzytkownika < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Niepoprawne id użytkownika: " + idUzytkownika));
        if (stareHaslo == noweHaslo)
            return ServiceResult<bool>.BadRequest(new ErrorItem("Hasła są takie same"));

        var bladHasla = SprawdzPoprawnoscHasla(noweHaslo);
        if (bladHasla != null) return ServiceResult<bool>.BadRequest(bladHasla);
        try
        {
            var wynikZBledami = await uzytkownikRepository.UpdateHaslo(idUzytkownika, stareHaslo, noweHaslo);
            // jeżeli jest w porządku
            if (wynikZBledami.Count == 0) return ServiceResult<bool>.Ok(true);
            // jeżeli nie jest w porządku
            var bledy = wynikZBledami.Select(e => new ErrorItem(e, nameof(noweHaslo)));
            return ServiceResult<bool>.BadRequest(bledy.ToArray());
        }catch(NieZnalezionoWBazieException e){
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<bool>> UpdateDaneKontaNaZewnetrznymSerwisie(int id, int? idNaZewnetrznymSerwisie, string? loginNaZewnetrznymSerwisie)
    {
        if(id < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Uzytkownik o id " + id + " nie istnieje"));
        if(idNaZewnetrznymSerwisie is < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id na zewnętrznym serwisie: " + idNaZewnetrznymSerwisie));
        try
        {
            var result = await uzytkownikRepository.UpdateDaneKontaNaZewnetrznymSerwisie(id, idNaZewnetrznymSerwisie, loginNaZewnetrznymSerwisie);
            return ServiceResult<bool>.Ok(result, 204);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> DeleteUzytkownik(int id)
    {
        if(id < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Niepoprawne id użytkownika: " + id));
        try
        {
            // nie trzeba tworzyć transakcji, bo jest ona w usuwaniu konta
            var result = await profilService.DeleteProfil(id);
            if (!result.Succeeded) return result;
            
            await uzytkownikRepository.DeleteUzytkownik(id);
        }catch(NieZnalezionoWBazieException e){
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
        return ServiceResult<bool>.Ok(true, 204);
    }
    private async Task<bool> CzyLoginIstnieje(int? idUzytkownika, string login) => await uzytkownikRepository.CzyLoginIstnieje(idUzytkownika, login);
    private async Task<bool> CzyEmailIstnieje(int? idUzytkownika, string email) => await uzytkownikRepository.CzyEmailIstnieje(idUzytkownika, email);

    // używane w create i update użytkownik
    private async Task<List<ErrorItem>> SprawdzPoprawnoscDanychPozaHaslem(int? id, string Login, string Email, string? NumerTelefonu, DateOnly DataUrodzenia)
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

        if (DataUrodzenia.Year < 1900 || DataUrodzenia.AddYears(18) > DateOnly.FromDateTime(DateTime.Now))
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