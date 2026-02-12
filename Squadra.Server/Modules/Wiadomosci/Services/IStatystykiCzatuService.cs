namespace Squadra.Server.Services;

public interface IStatystykiCzatuService
{
    public Task<ServiceResult<DateTime?>> GetDataNajnowszejWiadomosci(int idUzytkownika1, int idUzytkownika2);
    public Task<ServiceResult<bool>> CzySaNoweWiadomosciOdZnajomego(int idObecnegoUzytkownika, int idZnajomego);

}