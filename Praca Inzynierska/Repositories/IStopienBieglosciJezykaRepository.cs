using Praca_Inzynierska.DTO;

namespace Praca_Inzynierska.Repositories;

public interface IStopienBieglosciJezykaRepository
{
    public Task<ICollection<StopienBieglosciJezykaDto>> GetStopnieBieglosciJezyka();

    public Task<StopienBieglosciJezykaDto?> GetStopienBieglosciJezyka(int id);

}