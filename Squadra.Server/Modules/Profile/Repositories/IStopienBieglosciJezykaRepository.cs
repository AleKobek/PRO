
using Squadra.Server.Modules.Profile.DTO.JezykStopien;

namespace Squadra.Server.Modules.Profile.Repositories;

public interface IStopienBieglosciJezykaRepository
{
    public Task<ICollection<StopienBieglosciJezykaDto>> GetStopnieBieglosciJezyka();

    public Task<StopienBieglosciJezykaDto?> GetStopienBieglosciJezyka(int id);

}