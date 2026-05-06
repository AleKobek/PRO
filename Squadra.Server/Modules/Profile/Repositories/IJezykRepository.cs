using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Profile.Models;

namespace Squadra.Server.Modules.Profile.Repositories;

public interface IJezykRepository
{
    public Task<ICollection<Jezyk>> GetJezyki();

    public Task<Jezyk> GetJezyk(int id);

    public Task<ICollection<JezykOrazStopienDto>> GetJezykiProfilu(int id);

    public Task<ICollection<JezykOrazStopienDto>> ZmienJezykiProfilu(int profilId, ICollection<JezykProfiluCreateDto> noweJezyki);

}