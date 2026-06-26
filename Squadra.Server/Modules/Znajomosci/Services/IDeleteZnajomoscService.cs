using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Znajomosci.Services;

public interface IDeleteZnajomoscService
{
    public Task<ServiceResult<bool>> DeleteZnajomosc(int idUzytkownikaInicjujacego, int idUzytkownika2);
    public Task<ServiceResult<bool>> DeleteZnajomosciUzytkownika(int idUzytkownika);

}