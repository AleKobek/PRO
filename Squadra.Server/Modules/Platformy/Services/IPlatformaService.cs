using Squadra.Server.Modules.Platformy.Models;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Platformy.Services;

public interface IPlatformaService
{
    public Task<ServiceResult<ICollection<Platforma>>> GetPlatformy();

    public Task<ServiceResult<Platforma>> GetPlatformaById(int id);
}