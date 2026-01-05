using Squadra.Server.DTO.JezykStopien;
using Squadra.Server.Exceptions;
using Squadra.Server.Repositories;

namespace Squadra.Server.Services;

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