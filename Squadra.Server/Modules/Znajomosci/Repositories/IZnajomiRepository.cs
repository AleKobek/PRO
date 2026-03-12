using Squadra.Server.Modules.Znajomosci.Models;

namespace Squadra.Server.Modules.Znajomosci.Repositories;

public interface IZnajomiRepository
{
    public Task<ICollection<Znajomi>> GetZnajomiUzytkownika(int id);
    public Task<DateTime?> GetDataOstatniegoOtwarciaCzatu(int idSprawdzajacego, int idZnajomego);
    public Task<bool> CreateZnajomosc(int idUzytkownika1, int idUzytkownika2);
    public Task<bool> DeleteZnajomosc(int idUzytkownika1, int idUzytkownika2);

    public Task<bool> DeleteZnajomosciUzytkownika(int idUzytkownika);

    public Task<bool> CzyJestZnajomosc(int idUzytkownika1, int idUzytkownika2);

    public Task<bool> ZaktualizujOstatnieOtwarcieCzatu(int idOtwierajacego, int idZnajomego);

}