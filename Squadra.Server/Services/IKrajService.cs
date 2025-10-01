using Squadra.Server.DTO.KrajRegion;

namespace Squadra.Server.Services;

public interface IKrajService
{
    public Task<ICollection<KrajDto>> GetKraje();

    public Task<KrajDto> GetKraj(int id);
}