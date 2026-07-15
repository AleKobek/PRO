using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Drużyny.Services;

public interface IDeleteDruzynaService
{
    public Task<ServiceResult<bool>> UsunDruzyne(int idDruzyny, int idUsuwajacegoUzytkownika);
    public Task<ServiceResult<bool>> UsunDruzyneAdmin(int idDruzyny);
    public Task<ServiceResult<bool>> UsunWszystkieDruzynyDlaUzytkownika(int idUzytkownika);
}