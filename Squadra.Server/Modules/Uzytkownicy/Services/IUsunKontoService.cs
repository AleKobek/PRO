using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Uzytkownicy.Services;

public interface IUsunKontoService
{
    public Task<ServiceResult<bool>> UsunKonto(int id);
}