
namespace Squadra;

public interface IStopienBieglosciJezykaRepository
{
    public Task<ICollection<StopienBieglosciJezykaDto>> GetStopnieBieglosciJezyka();

    public Task<StopienBieglosciJezykaDto?> GetStopienBieglosciJezyka(int id);

}