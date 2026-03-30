using Squadra.Server.Modules.Platformy.Models;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.WspieraneGry.DTO;
using Squadra.Server.Modules.WspieraneGry.Models;

namespace Squadra.Server.Modules.WspieraneGry.Services;

public interface IWspieranaGraService
{
    public Task<ServiceResult<ICollection<WspieranaGra>>> GetWspieraneGry();
    public Task<ServiceResult<WspieranaGra>> GetWspieranaGra(int idGry);
    public Task<ServiceResult<ICollection<WspieranaGra>>> GetWspieraneGryMinInfo();
    public Task<ServiceResult<ICollection<GraZPlatformaDTO>>> GetWspieraneGryZPlatformami();
    public Task<ServiceResult<ICollection<Platforma>>> GetPlatformyGry(int idGry);
}