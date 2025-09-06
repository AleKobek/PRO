
namespace Squadra;

public interface IUzytkownikRepository
{
    public Task<ICollection<UzytkownikOrazProfilDto>> GetUzytkownicy();
    
    public Task<UzytkownikOrazProfilDto?> GetUzytkownik(int id);

    public Task<Uzytkownik?> AddUzytkownik(Uzytkownik uzytkownik);

    public Task<Uzytkownik?> UpdateUzytkownik(Uzytkownik uzytkownik);

}