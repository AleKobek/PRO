using Squadra.Server.Modules.Wiadomosci.DTO;

namespace Squadra.Server.Modules.Wiadomosci.Repositories;

public interface IWiadomoscRepository
{
    public Task<WiadomoscDto> GetWiadomosc(int id);
    public Task<ICollection<WiadomoscDto>> GetWiadomosci(int idUzytkownika1, int idUzytkownika2);
    public Task<DateTime?> GetDataNajnowszejWiadomosci(int idUzytkownika1, int idUzytkownika2);
    public Task<bool> CreateWiadomosc(int idOdbiorcy, WiadomoscCreateDto wiadomosc, int idNadawcy);

    public Task<bool> DeleteWiadomosciUzytkownikow(int idUzytkownika1, int idUzytkownika2);

}