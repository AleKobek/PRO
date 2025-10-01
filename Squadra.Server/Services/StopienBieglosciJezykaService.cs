using Squadra.Server.DTO.JezykStopien;
using Squadra.Server.Exceptions;
using Squadra.Server.Repositories;

namespace Squadra.Server.Services;

public class StopienBieglosciJezykaService(IStopienBieglosciJezykaRepository stopienBieglosciJezykaRepository) : IStopienBieglosciJezykaService
{
    public async Task<ICollection<StopienBieglosciJezykaDto>> GetStopnieBieglosciJezyka()
    {
        return await stopienBieglosciJezykaRepository.GetStopnieBieglosciJezyka();
    }

    public async Task<StopienBieglosciJezykaDto?> GetStopienBieglosciJezyka(int id)
    {
        if (id < 1) throw new NieZnalezionoWBazieException("Stopien biegłości języka o id " + id + " nie istnieje");
        return await stopienBieglosciJezykaRepository.GetStopienBieglosciJezyka(id);
    }

}