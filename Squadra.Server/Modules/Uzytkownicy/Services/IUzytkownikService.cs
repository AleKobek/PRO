using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Uzytkownicy.DTO.Uzytkownik;

namespace Squadra.Server.Modules.Uzytkownicy.Services;

public interface IUzytkownikService
{
    public Task<ServiceResult<ICollection<UzytkownikResDto>>> GetUzytkownicy();
    public Task<ServiceResult<UzytkownikResDto>> GetUzytkownik(int id);

    public Task<ServiceResult<UzytkownikResDto>> GetUzytkownik(string login);

    Task<ServiceResult<bool>> CreateUzytkownik(UzytkownikCreateDto uzytkownik);

    public Task<ServiceResult<bool>> UpdateUzytkownik(int id, UzytkownikUpdateDto dto);

    public Task<ServiceResult<bool>> UpdateHaslo(int idUzytkownika, string stareHaslo, string noweHaslo);


    public Task<ServiceResult<bool>> DeleteUzytkownik(int id);

}