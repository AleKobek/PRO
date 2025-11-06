using System.Text.RegularExpressions;
using Squadra.Server.DTO.Uzytkownik;
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
        if(id < 1) return ServiceResult<UzytkownikResDto>.NotFound(new ErrorItem("Uzytkownik o id " + id + " nie istnieje"));
        
        return ServiceResult<UzytkownikResDto>.Ok(await uzytkownikRepository.GetUzytkownik(id));
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
        var bledy = await SprawdzPoprawnoscDanych(
            uzytkownik.Login,
            uzytkownik.Haslo,
            uzytkownik.Email,
            uzytkownik.NumerTelefonu,
            uzytkownik.DataUrodzenia
        );

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
    public Task<UzytkownikResDto> Update(UzytkownikUpdateDto dto)
    {
        return uzytkownikRepository.UpdateUzytkownik(dto);
    }
    public Task Delete(int id) => uzytkownikRepository.DeleteUzytkownik(id);
    private Task<bool> CzyLoginIstnieje(string login) => uzytkownikRepository.CzyLoginIstnieje(login);
    private Task<bool> CzyEmailIstnieje(string email) => uzytkownikRepository.CzyEmailIstnieje(email);

    // tutaj wyciągamy aby użyć też w update
    private async Task<List<ErrorItem>> SprawdzPoprawnoscDanych(string Login, string Haslo, string Email, string? NumerTelefonu, DateOnly DataUrodzenia)
    {
        var bledy = new List<ErrorItem>();
        
        if (await CzyLoginIstnieje(Login))
            bledy.Add(new ErrorItem("Taki login już istnieje.", nameof(Login), "LoginIstnieje"));

        if (await CzyEmailIstnieje(Email))
            bledy.Add(new ErrorItem("Taki email już istnieje.", nameof(Email), "EmailIstnieje"));
        
        var re = new Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*])(?=.{8,128})");
        if (!re.IsMatch(Haslo))
            bledy.Add(new ErrorItem("Niepoprawne hasło.", nameof(Haslo), "NiepoprawneHaslo"));
        
        re = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
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
}