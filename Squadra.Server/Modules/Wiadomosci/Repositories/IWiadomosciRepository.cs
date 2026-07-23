using Squadra.Server.Modules.Wiadomosci.DTO;

namespace Squadra.Server.Modules.Wiadomosci.Repositories;

public interface IWiadomosciRepository
{
    public Task<WiadomoscDto> GetWiadomosc(int id);
    public Task<ICollection<WiadomoscDto>> GetWiadomosciPrywatne(int idUzytkownika1, int idUzytkownika2);
    public Task<ICollection<WiadomoscDto>> GetWiadomosciNaCzacieDruzyny(int idDruzyny);
    public Task<DateTime?> GetDataNajnowszejWiadomosciPrywatnej(int idUzytkownika1, int idUzytkownika2);
    public Task<DateTime?> GetDataNajnowszejWiadomosciWDruzynie(int idDruzyny);
    public Task<bool> CreateWiadomosc(int idOdbiorcy, WiadomoscCreateDto wiadomosc, int idNadawcy);
    public Task<bool> DeleteWiadomosciPrywatneUzytkownikow(int idUzytkownika1, int idUzytkownika2);
    public Task<bool> DeleteWiadomosciDruzyny(int idDruzyny);
}