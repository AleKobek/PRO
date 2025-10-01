using Squadra.Server.DTO.JezykStopien;

namespace Squadra.Server.Repositories;

public interface IJezykRepository
{
    public Task<ICollection<JezykDto>> GetJezyki();

    public Task<JezykDto?> GetJezyk(int id);

    public Task<ICollection<JezykOrazStopienDto>> GetJezykiProfilu(int id);

    public Task<ICollection<JezykOrazStopienDto>> ZmienJezykiProfilu(int profilId, ICollection<JezykOrazStopienDto> noweJezyki);

}