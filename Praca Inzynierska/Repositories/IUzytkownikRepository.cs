using Praca_Inzynierska.DTO;

namespace Praca_Inzynierska.Repositories;

public interface IUzytkownikRepository
{
    public Task<ICollection<UzytkownikOrazProfilDto>> GetUzytkownicy();
    
    public Task<UzytkownikOrazProfilDto?> GetUzytkownik(int id);

}