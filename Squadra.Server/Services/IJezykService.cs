using Squadra.Server.DTO.JezykStopien;

namespace Squadra.Server.Services;

public interface IJezykService
{
    public Task<ServiceResult<ICollection<JezykDto>>> GetJezyki();

    public Task<ServiceResult<JezykDto?>> GetJezyk(int id);

    public Task<ServiceResult<ICollection<JezykOrazStopienDto>>> GetJezykiProfilu(int id);

    public Task<ServiceResult<ICollection<JezykOrazStopienDto>>> ZmienJezykiProfilu(int profilId,
        ICollection<JezykProfiluCreateDto> noweJezyki);
}