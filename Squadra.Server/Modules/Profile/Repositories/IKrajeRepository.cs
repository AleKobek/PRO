using Squadra.Server.Modules.Profile.DTO.KrajRegion;

namespace Squadra.Server.Modules.Profile.Repositories;

public interface IKrajeRepository
{
    public Task<ICollection<KrajDto>> GetKraje();
    
    public Task<KrajDto> GetKraj(int id);

    public KrajDto GetKrajDomyslny();
}