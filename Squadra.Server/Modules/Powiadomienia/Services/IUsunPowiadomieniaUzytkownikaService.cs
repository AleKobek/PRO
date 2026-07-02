using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Powiadomienia.Services;

public interface IUsunPowiadomieniaUzytkownikaService
{
    public Task<ServiceResult<bool>> DeletePowiadomieniaZwiazaneZUzytkownikiem(int idUzytkownika);
}