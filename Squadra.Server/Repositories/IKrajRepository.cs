
using Squadra.Server.DTO.KrajRegion;

namespace Squadra.Server.Repositories;

public interface IKrajRepository
{
    public Task<ICollection<KrajDto>> GetKraje();
    
    public Task<KrajDto> GetKraj(int id);

    public KrajDto GetKrajDomyslny();
}