
namespace Squadra;

public interface IKrajRepository
{
    public Task<ICollection<KrajDto>> GetKraje();
    
    public Task<KrajDto> GetKraj(int id);

    public KrajDto GetKrajDomyslny();
}