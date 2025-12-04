using Squadra.Server.DTO.Uzytkownik;

namespace Squadra.Server.Services;

public interface IUzytkownikService
{
    public Task<ServiceResult<ICollection<UzytkownikResDto>>> GetUzytkownicy();
    public Task<ServiceResult<UzytkownikResDto>> GetUzytkownik(int id);

    Task<ServiceResult<bool>> CreateUzytkownik(UzytkownikCreateDto uzytkownik);

    public Task<ServiceResult<bool>> UpdateUzytkownik(int id, UzytkownikUpdateDto dto);

    public Task<ServiceResult<bool>> UpdateHaslo(int idUzytkownika, string stareHaslo, string noweHaslo);


    public Task<ServiceResult<bool>> DeleteUzytkownik(int id);

}