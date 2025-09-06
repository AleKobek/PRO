namespace Squadra;

public interface IJezykRepository
{
    public Task<ICollection<JezykDto>> GetJezyki();

    public Task<JezykDto?> GetJezyk(int id);

    public Task<ICollection<JezykOrazStopienDto>> GetJezykiUzytkownika(int id);

}