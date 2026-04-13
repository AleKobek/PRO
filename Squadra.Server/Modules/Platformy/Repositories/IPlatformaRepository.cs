using Squadra.Server.Modules.Platformy.DTO;
using Squadra.Server.Modules.Platformy.Models;

namespace Squadra.Server.Modules.Platformy.Repositories;

public interface IPlatformaRepository
{
    public Task<ICollection<Platforma>> GetPlatformy();
    public Task<Platforma> GetPlatforma(int id);
    public Task<ICollection<PlatformaUzytkownikaDTO>> GetPlatformyUzytkownika(int idUzytkownika);
    public Task<bool> UpdatePlatformyUzytkownika(int idUzytkownika, List<UzytkownikPlatforma> nowePlatformy);
    public Task<bool> UsunPlatformyUzytkownika(int idUzytkownika);
    public Task<bool> CreatePlatforma(int id, string nazwa, byte[] logo);
}