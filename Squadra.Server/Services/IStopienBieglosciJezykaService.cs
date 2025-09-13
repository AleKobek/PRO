namespace Squadra.Services;

public interface IStopienBieglosciJezykaService
{
    public Task<ICollection<StopienBieglosciJezykaDto>> GetStopnieBieglosciJezyka();

    public Task<StopienBieglosciJezykaDto?> GetStopienBieglosciJezyka(int id);
}