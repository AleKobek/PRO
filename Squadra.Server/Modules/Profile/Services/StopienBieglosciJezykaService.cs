using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Profile.Models;
using Squadra.Server.Modules.Profile.Repositories;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Profile.Services;

public class StopienBieglosciJezykaService(IStopienBieglosciJezykaRepository stopienBieglosciJezykaRepository) : IStopienBieglosciJezykaService
{
    public async Task<ServiceResult<ICollection<StopienBieglosciJezyka>>> GetStopnieBieglosciJezyka()
    {
        return ServiceResult<ICollection<StopienBieglosciJezyka>>.Ok(await stopienBieglosciJezykaRepository.GetStopnieBieglosciJezyka());
    }

    public async Task<ServiceResult<StopienBieglosciJezyka>> GetStopienBieglosciJezyka(int id)
    {
        if (id < 1) return ServiceResult<StopienBieglosciJezyka>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator stopnia bieglosci jezyka: " + id));
        try
        {
            return ServiceResult<StopienBieglosciJezyka>.Ok(
                await stopienBieglosciJezykaRepository.GetStopienBieglosciJezyka(id));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<StopienBieglosciJezyka>.NotFound(new ErrorItem(e.Message));
        }
    }

}