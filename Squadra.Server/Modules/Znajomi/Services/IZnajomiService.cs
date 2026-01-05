using Squadra.Server.DTO.Profil;

namespace Squadra.Server.Services;

public interface IZnajomiService
{
    public Task<ServiceResult<ICollection<ProfilGetResDto>>> GetZnajomiUzytkownika(int id);
    public Task<ServiceResult<bool>> CreateZnajomosc(int idUzytkownika1, int idUzytkownika2);
    public Task<ServiceResult<bool>> DeleteZnajomosc(int idUzytkownikaInicjujacego, int idUzytkownika2);
}