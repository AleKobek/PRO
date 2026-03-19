using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Platformy.DTO;
using Squadra.Server.Modules.Platformy.Repositories;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Platformy.Services;

public class UzytkownikPlatformaService(IUzytkownikPlatformaRepository uzytkownikPlatformaRepository) : IUzytkownikPlatformaService
{
    public async Task<ServiceResult<ICollection<PlatformaUzytkownikaDTO>>> GetPlatformyUzytkownika(int idUzytkownika)
    {
        try
        {
            var platformy = await uzytkownikPlatformaRepository.GetPlatformyUzytkownika(idUzytkownika);
            return ServiceResult<ICollection<PlatformaUzytkownikaDTO>>.Ok(platformy);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<ICollection<PlatformaUzytkownikaDTO>>.NotFound(new ErrorItem(e.Message));
        }
    }
}