using Squadra.Server.DTO.Uzytkownik;

namespace Squadra.Server.Services;

public interface IUzytkownikService
{
    public Task<ServiceResult<ICollection<UzytkownikResDto>>> GetUzytkownicy();
    public Task<ServiceResult<UzytkownikResDto>> GetUzytkownik(int id);

    Task<ServiceResult<UzytkownikResDto>> CreateUzytkownik(UzytkownikCreateDto uzytkownik);

    
}