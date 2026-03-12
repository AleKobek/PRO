using Squadra.Server.Modules.Platformy.DTO;

namespace Squadra.Server.Modules.Platformy.Repositories;

public interface IUzytkownikPlatformaRepository
{
    public Task<ICollection<PlatformaUzytkownikaDTO>> GetPlatformyUzytkownika(int idUzytkownika);
}