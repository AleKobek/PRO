
namespace Squadra;

public interface IUzytkownikRepository
{
    public Task<ICollection<UzytkownikDto>> GetUzytkownicy();
    
    public Task<UzytkownikDto?> GetUzytkownik(int id);

    public Task<Uzytkownik?> AddUzytkownik(Uzytkownik uzytkownik);

    public Task<Uzytkownik?> UpdateUzytkownik(Uzytkownik uzytkownik);

}