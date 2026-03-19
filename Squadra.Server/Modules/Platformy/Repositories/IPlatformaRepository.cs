using Squadra.Server.Modules.Platformy.Models;

namespace Squadra.Server.Modules.Platformy.Repositories;

public interface IPlatformaRepository
{
    public Task<ICollection<Platforma>> GetPlatformy();
    public Task<Platforma> GetPlatformaById(int id);
}