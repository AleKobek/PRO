
using Squadra.Server.DTO.JezykStopien;

namespace Squadra.Server.Repositories;

public interface IStopienBieglosciJezykaRepository
{
    public Task<ICollection<StopienBieglosciJezykaDto>> GetStopnieBieglosciJezyka();

    public Task<StopienBieglosciJezykaDto?> GetStopienBieglosciJezyka(int id);

}