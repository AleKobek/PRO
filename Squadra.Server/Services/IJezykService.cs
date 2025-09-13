namespace Squadra.Services;

public interface IJezykService
{
    public Task<ICollection<JezykDto>> GetJezyki();

    public Task<JezykDto?> GetJezyk(int id);

    public Task<ICollection<JezykOrazStopienDto>> GetJezykiProfilu(int id);

    public Task<ICollection<JezykOrazStopienDto>> ZmienJezykiProfilu(int profilId,
        ICollection<JezykOrazStopienDto> noweJezyki);
}