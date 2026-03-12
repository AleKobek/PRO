using Squadra.Server.Modules.Profile.DTO.JezykStopien;

namespace Squadra.Server.Modules.Profile.Repositories;

public interface IJezykRepository
{
    public Task<ICollection<JezykDto>> GetJezyki();

    public Task<JezykDto?> GetJezyk(int id);

    public Task<ICollection<JezykOrazStopienDto>> GetJezykiProfilu(int id);

    public Task<ICollection<JezykOrazStopienDto>> ZmienJezykiProfilu(int profilId, ICollection<JezykProfiluCreateDto> noweJezyki);

}