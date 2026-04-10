using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.IntegracjeZewnetrzne.Services;

public interface IIntegracjeZewnetrzneService
{
    public Task<ServiceResult<bool>> ZintegrujKonto(int idUzytkownika, string login, string haslo);
    public Task<ServiceResult<bool>> UpdateCaleZintegrowaneDaneUzytkownika(int idUzytkownika);
}