namespace Squadra.Services;

public interface IStatusService
{
    public Task<ICollection<StatusDto>> GetStatusy();

    public Task<StatusDto?> GetStatus(int id);
    
    public StatusDto GetStatusDomyslny();
}