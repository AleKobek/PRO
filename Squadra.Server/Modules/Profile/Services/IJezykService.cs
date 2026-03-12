using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Profile.Services;

public interface IJezykService
{
    public Task<ServiceResult<ICollection<JezykDto>>> GetJezyki();

    public Task<ServiceResult<JezykDto?>> GetJezyk(int id);

    public Task<ServiceResult<ICollection<JezykOrazStopienDto>>> GetJezykiProfilu(int id);

    public Task<ServiceResult<ICollection<JezykOrazStopienDto>>> ZmienJezykiProfilu(int profilId,
        ICollection<JezykProfiluCreateDto> noweJezyki);
}