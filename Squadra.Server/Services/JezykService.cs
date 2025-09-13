using Squadra.Exceptions;

namespace Squadra.Services;

public class JezykService(IJezykRepository jezykRepository) : IJezykService
{
    public async Task<ICollection<JezykDto>> GetJezyki()
    {
        return await jezykRepository.GetJezyki();
    }

    public async Task<JezykDto?> GetJezyk(int id)
    {
        if (id < 1) throw new NieZnalezionoWBazieException("Jezyk o id " + id + " nie istnieje");
        return await jezykRepository.GetJezyk(id);
    }

    public async Task<ICollection<JezykOrazStopienDto>> GetJezykiProfilu(int id)
    {
        if(id < 1) throw new NieZnalezionoWBazieException("Profil o id " + id + " nie istnieje");
        return await jezykRepository.GetJezykiProfilu(id);
    }

    public async Task<ICollection<JezykOrazStopienDto>> ZmienJezykiProfilu(int profilId,
        ICollection<JezykOrazStopienDto> noweJezyki)
    {
        if(profilId < 1) throw new NieZnalezionoWBazieException("Profil o id " + profilId + " nie istnieje");
        return await jezykRepository.ZmienJezykiProfilu(profilId, noweJezyki);
    }

}