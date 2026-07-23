using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Wiadomosci.DTO;

namespace Squadra.Server.Modules.Wiadomosci.Services;

public interface IWiadomosciService
{
    public Task<ServiceResult<WiadomoscDto>> GetWiadomosc(int idWiadomosci, int idObecnegoUzytkownika);
    public Task<ServiceResult<ICollection<WiadomoscDto>>> GetWiadomosciPrywatne(int idUzytkownika1, int idUzytkownika2);
    public Task<ServiceResult<CzatDruzynowyDto>> GetWiadomosciNaCzacieDruzyny(int idDruzyny, int idCzytajacego);
    public Task<ServiceResult<bool>> CreateWiadomoscPrywatna(int idOdbiorcy, string tresc, int idObecnegoUzytkownika);
    public Task<ServiceResult<bool>> CreateWiadomoscDruzynowa(int idDruzyny, string tresc, int idObecnegoUzytkownika);
    public Task<ServiceResult<bool>> DeleteWiadomosciPrywatneUzytkownikow(int idUzytkownika1, int idUzytkownika2);
    public Task<ServiceResult<bool>> DeleteWiadomosciDruzyny(int idDruzyny);
}