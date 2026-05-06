
using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Profile.Models;

namespace Squadra.Server.Modules.Profile.Repositories;

public interface IStopienBieglosciJezykaRepository
{
    public Task<ICollection<StopienBieglosciJezyka>> GetStopnieBieglosciJezyka();

    public Task<StopienBieglosciJezyka> GetStopienBieglosciJezyka(int id);

}