using Squadra.Server.DTO.Wiadomosc;

namespace Squadra.Server.Repositories;

public interface IWiadomoscRepository
{
    public Task<WiadomoscDto> GetWiadomosc(int id);
    public Task<ICollection<WiadomoscDto>> GetWiadomosci(int idUzytkownika1, int idUzytkownika2);

    public Task<bool> CreateWiadomosc(WiadomoscCreateDto wiadomosc, int idNadawcy);

    public Task<bool> DeleteWiadomosciUzytkownikow(int idUzytkownika1, int idUzytkownika2);

}