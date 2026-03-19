using Squadra.Server.Modules.Platformy.DTO;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Platformy.Services;

public interface IUzytkownikPlatformaService
{
    public Task<ServiceResult<ICollection<PlatformaUzytkownikaDTO>>> GetPlatformyUzytkownika(int idUzytkownika);
}