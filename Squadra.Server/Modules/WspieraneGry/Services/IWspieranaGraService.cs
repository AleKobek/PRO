using Squadra.Server.Modules.Platformy.DTO;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.WspieraneGry.DTO;

namespace Squadra.Server.Modules.WspieraneGry.Services;

public interface IWspieranaGraService
{
    public Task<ServiceResult<ICollection<WspieranaGraDto>>> GetWspieraneGry();
    public Task<ServiceResult<WspieranaGraDto>> GetWspieranaGra(int idGry);
    public Task<ServiceResult<ICollection<MinInfoWspieranaGraDTO>>> GetWspieraneGryMinInfo();
    public Task<ServiceResult<ICollection<GraZPlatformaDTO>>> GetWspieraneGryZPlatformami();
    public Task<ServiceResult<ICollection<GraZPlatformaDoSelectDto>>> GetWspieraneGryZPlatformamiDoSelect();
    public Task<ServiceResult<ICollection<PlatformaDto>>> GetPlatformyGry(int idGry);
    public Task<ServiceResult<ICollection<PlatformaDto>>> GetPlatformyGryUzytkownika(int idGry, int idUzytkownika);
}