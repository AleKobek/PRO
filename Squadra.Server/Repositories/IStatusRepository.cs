using Squadra.Server.DTO.Status;

namespace Squadra.Server.Repositories;

public interface IStatusRepository
{
    public Task<ICollection<StatusDto>> GetStatusy();
    
    public Task<StatusDto?> GetStatus(int id);

    public Task<int?> GetIdStatusu(string nazwa);

    public StatusDto GetStatusDomyslny();

}