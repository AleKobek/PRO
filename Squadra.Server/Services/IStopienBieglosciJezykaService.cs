using Squadra.Server.DTO.JezykStopien;

namespace Squadra.Server.Services;

public interface IStopienBieglosciJezykaService
{
    public Task<ICollection<StopienBieglosciJezykaDto>> GetStopnieBieglosciJezyka();

    public Task<StopienBieglosciJezykaDto?> GetStopienBieglosciJezyka(int id);
}