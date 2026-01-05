using Squadra.Server.DTO.JezykStopien;

namespace Squadra.Server.Services;

public interface IStopienBieglosciJezykaService
{
    public Task<ServiceResult<ICollection<StopienBieglosciJezykaDto>>> GetStopnieBieglosciJezyka();

    public Task<ServiceResult<StopienBieglosciJezykaDto?>> GetStopienBieglosciJezyka(int id);
}