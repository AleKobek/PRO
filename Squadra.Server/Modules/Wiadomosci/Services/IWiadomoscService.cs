using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Wiadomosci.DTO;

namespace Squadra.Server.Modules.Wiadomosci.Services;

public interface IWiadomoscService
{
    public Task<ServiceResult<WiadomoscDto>> GetWiadomosc(int idWiadomosci, int idObecnegoUzytkownika);
    public Task<ServiceResult<ICollection<WiadomoscDto>>> GetWiadomosci(int idUzytkownika1, int idUzytkownika2);
    public Task<ServiceResult<bool>> CreateWiadomosc(int idOdbiorcy, WiadomoscCreateDto wiadomosc, int idObecnegoUzytkownika);
    public Task<ServiceResult<bool>> DeleteWiadomosciUzytkownikow(int idUzytkownika1, int idUzytkownika2);
}