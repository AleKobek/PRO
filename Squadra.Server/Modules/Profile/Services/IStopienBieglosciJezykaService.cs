using Squadra.Server.Modules.Profile.Models;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Profile.Services;

public interface IStopienBieglosciJezykaService
{
    public Task<ServiceResult<ICollection<StopienBieglosciJezyka>>> GetStopnieBieglosciJezyka();

    public Task<ServiceResult<StopienBieglosciJezyka>> GetStopienBieglosciJezyka(int id);
}