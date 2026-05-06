using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Profile.Models;
using Squadra.Server.Modules.Profile.Repositories;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Profile.Services;

public class JezykService(IJezykRepository jezykRepository) : IJezykService
{
    public async Task<ServiceResult<ICollection<Jezyk>>> GetJezyki()
    {
        return ServiceResult<ICollection<Jezyk>>.Ok(await jezykRepository.GetJezyki());
    }

    public async Task<ServiceResult<Jezyk>> GetJezyk(int id)
    {
        try
        {
            return id < 1
                ? ServiceResult<Jezyk>.BadRequest(new ErrorItem("Nieprawidłowe id jezyka: " + id))
                : ServiceResult<Jezyk>.Ok(await jezykRepository.GetJezyk(id));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<Jezyk>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<ICollection<JezykOrazStopienDto>>> GetJezykiProfilu(int id)
    {
        try{
            return id < 1
                ? ServiceResult<ICollection<JezykOrazStopienDto>>.BadRequest(
                    new ErrorItem("Nieprawidłowe id profilu: " + id))
                : ServiceResult<ICollection<JezykOrazStopienDto>>.Ok(await jezykRepository.GetJezykiProfilu(id));
        }catch(NieZnalezionoWBazieException e){
            return ServiceResult<ICollection<JezykOrazStopienDto>>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<ICollection<JezykOrazStopienDto>>> ZmienJezykiProfilu(int profilId,
        ICollection<JezykProfiluCreateDto> noweJezyki)
    {
        try
        {
            return profilId < 1
                ? ServiceResult<ICollection<JezykOrazStopienDto>>.BadRequest(
                    new ErrorItem("Nieprawidłowe id profilu: " + profilId))
                : ServiceResult<ICollection<JezykOrazStopienDto>>.Ok(
                    await jezykRepository.ZmienJezykiProfilu(profilId, noweJezyki));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<ICollection<JezykOrazStopienDto>>.NotFound(new ErrorItem(e.Message));
        }
    }

}