namespace Squadra.Services;

public class KrajService (IKrajRepository krajRepository) : IKrajService
{
    public async Task<ICollection<KrajDto>> GetKraje()
    {
        return await krajRepository.GetKraje();
    }

    public async Task<KrajDto?> GetKraj(int id)
    {
        if (id < 1) throw new Exception("Kraj o id " + id + " nie istnieje");
        
        return await krajRepository.GetKraj(id);
    }
}