using Squadra.Server.DTO.Wiadomosc;

namespace Squadra.Server.Services;

public interface IWiadomoscService
{
    public Task<ServiceResult<WiadomoscDto>> GetWiadomosc(int idWiadomosci, int idObecnegoUzytkownika);
    public Task<ServiceResult<ICollection<WiadomoscDto>>> GetWiadomosci(int idUzytkownika1, int idUzytkownika2);
    public Task<ServiceResult<bool>> CreateWiadomosc(WiadomoscCreateDto wiadomosc);
    public Task<ServiceResult<bool>> DeleteWiadomosciUzytkownikow(int idUzytkownika1, int idUzytkownika2);
}