using Squadra.Server.DTO.Uzytkownik;

namespace Squadra.Server.Services;

public interface IUzytkownikService
{
    public Task<ICollection<UzytkownikResDto>> GetUzytkownicy();
    public Task<UzytkownikResDto> GetUzytkownik(int id);
}