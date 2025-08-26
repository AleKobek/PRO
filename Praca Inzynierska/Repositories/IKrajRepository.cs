using Praca_Inzynierska.DTO;
using Praca_Inzynierska.Models;

namespace Praca_Inzynierska.Repositories;

public interface IKrajRepository
{
    public Task<ICollection<KrajDto>> GetKraje();
    
    public Task<KrajDto?> GetKraj(int id);
}