using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Profile.Services;

public interface IStopienBieglosciJezykaService
{
    public Task<ServiceResult<ICollection<StopienBieglosciJezykaDto>>> GetStopnieBieglosciJezyka();

    public Task<ServiceResult<StopienBieglosciJezykaDto?>> GetStopienBieglosciJezyka(int id);
}