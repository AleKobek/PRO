
namespace Squadra;

public interface IUzytkownikRepository
{
    public Task<ICollection<UzytkownikResDto>> GetUzytkownicy();
    
    public Task<UzytkownikResDto> GetUzytkownik(int id);

    public Task<UzytkownikResDto> CreateUzytkownik(UzytkownikCreateDto uzytkownik);

    public Task<UzytkownikResDto> UpdateUzytkownik(UzytkownikUpdateDto uzytkownik);

    public Task DeleteUzytkownik(int id);
    public Task<UzytkownikResDto> UpdateStatus(int id, int idStatus);
}