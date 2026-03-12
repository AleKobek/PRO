using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Wiadomosci.Services;

public interface IStatystykiCzatuService
{
    public Task<ServiceResult<DateTime?>> GetDataNajnowszejWiadomosci(int idUzytkownika1, int idUzytkownika2);
    public Task<ServiceResult<bool>> CzySaNoweWiadomosciOdZnajomego(int idObecnegoUzytkownika, int idZnajomego);
    public Task<ServiceResult<bool>> CzySaNoweWiadomosciOdZnajomych(int idObecnegoUzytkownika);
}