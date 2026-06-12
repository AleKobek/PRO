using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Powiadomienia.Services;

public interface IRozpatrzPowiadomienieService
{
    public Task<ServiceResult<bool>> RozpatrzPowiadomienie(int id, bool? czyZaakceptowane, int idUzytkownika);
}