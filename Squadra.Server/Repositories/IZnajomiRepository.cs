using Squadra.Server.DTO.Profil;

namespace Squadra.Server.Repositories;

public interface IZnajomiRepository
{
    public Task<ICollection<ProfilGetResDto>> GetZnajomiUzytkownika(int id);
    public Task<bool> CreateZnajomosc(int idUzytkownika1, int idUzytkownika2);
    public Task<bool> DeleteZnajomosc(int idUzytkownika1, int idUzytkownika2);

}