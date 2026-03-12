using Squadra.Server.Modules.Profile.DTO.KrajRegion;

namespace Squadra.Server.Modules.Profile.Repositories;

public interface IKrajRepository
{
    public Task<ICollection<KrajDto>> GetKraje();
    
    public Task<KrajDto> GetKraj(int id);

    public KrajDto GetKrajDomyslny();
}