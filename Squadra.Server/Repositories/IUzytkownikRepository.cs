using Squadra.Server.DTO.Status;
using Squadra.Server.DTO.Uzytkownik;

namespace Squadra.Server.Repositories;

public interface IUzytkownikRepository
{
    public Task<ICollection<UzytkownikResDto>> GetUzytkownicy();
    
    public Task<UzytkownikResDto> GetUzytkownik(int id);

    public Task<UzytkownikResDto> GetUzytkownik(string login);

    public Task<bool> CreateUzytkownik(UzytkownikCreateDto uzytkownik);

    public Task<bool> UpdateUzytkownik(int id, UzytkownikUpdateDto uzytkownik);

    public Task<DateTime?> GetOstatniaAktywnoscUzytkownika(int id);

    public Task<List<string>> UpdateHaslo(int idUzytkownika, string stareHaslo, string noweHaslo);


    public Task DeleteUzytkownik(int id);
    
    Task<bool> CzyLoginIstnieje(int? id, string login);
    
    Task<bool> CzyEmailIstnieje(int? id, string email);
}