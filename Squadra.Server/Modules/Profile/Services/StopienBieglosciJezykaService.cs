using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Profile.Repositories;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Profile.Services;

public class StopienBieglosciJezykaService(IStopienBieglosciJezykaRepository stopienBieglosciJezykaRepository) : IStopienBieglosciJezykaService
{
    public async Task<ServiceResult<ICollection<StopienBieglosciJezykaDto>>> GetStopnieBieglosciJezyka()
    {
        return ServiceResult<ICollection<StopienBieglosciJezykaDto>>.Ok(await stopienBieglosciJezykaRepository.GetStopnieBieglosciJezyka());
    }

    public async Task<ServiceResult<StopienBieglosciJezykaDto?>> GetStopienBieglosciJezyka(int id)
    {
        if (id < 1) return ServiceResult<StopienBieglosciJezykaDto?>.NotFound(new ErrorItem("Stopien biegłości języka o id " + id + " nie istnieje"));
        return ServiceResult<StopienBieglosciJezykaDto?>.Ok(await stopienBieglosciJezykaRepository.GetStopienBieglosciJezyka(id));
    }

}