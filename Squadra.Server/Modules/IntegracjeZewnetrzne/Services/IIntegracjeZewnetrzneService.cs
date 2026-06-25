using Squadra.Server.Modules.IntegracjeZewnetrzne.DTO;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.IntegracjeZewnetrzne.Services;

public interface IIntegracjeZewnetrzneService
{
    public Task<ServiceResult<ZintegrujKontoRes>> ZintegrujKonto(int idUzytkownika, string login, string haslo);
    public Task<ServiceResult<bool>> PrzerwijIntegracjeUzytkownika(int idUzytkownika, bool czyPrzyUsuwaniuKonta);
    public Task<ServiceResult<bool>> UpdateCaleZintegrowaneDaneUzytkownika(int idUzytkownika);
}