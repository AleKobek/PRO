using Squadra.Server.Modules.Platformy.Models;
using Squadra.Server.Modules.WspieraneGry.DTO;
using Squadra.Server.Modules.WspieraneGry.Models;

namespace Squadra.Server.Modules.WspieraneGry.Repositories;

public interface IWspieranaGraRepository
{
    public Task<ICollection<WspieranaGra>> GetWspieraneGry();
    public Task<WspieranaGra> GetWspieranaGra(int id);
    public Task<ICollection<WspieranaGra>> GetWspieraneGryMinInfo();
    public Task<ICollection<Platforma>> GetPlatformyGry(int idGry);
    public Task<ICollection<GraZPlatformaDTO>> GetWspieraneGryZPlatformami();
}