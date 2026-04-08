using Squadra.Server.Modules.Platformy.DTO;
using Squadra.Server.Modules.Platformy.Models;

namespace Squadra.Server.Modules.Platformy.Repositories;

public interface IPlatformaRepository
{
    public Task<ICollection<Platforma>> GetPlatformy();
    public Task<Platforma> GetPlatforma(int id);
    public Task<ICollection<PlatformaUzytkownikaDTO>> GetPlatformyUzytkownika(int idUzytkownika);
    public Task<ICollection<PlatformaUzytkownikaDTO>> UpdatePlatformyUzytkownika(int idUzytkownika);
    public Task<bool> UsunPlatformyUzytkownika(int idUzytkownika);
}