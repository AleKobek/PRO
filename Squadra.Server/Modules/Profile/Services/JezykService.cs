using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Profile.Repositories;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Profile.Services;

public class JezykService(IJezykRepository jezykRepository) : IJezykService
{
    public async Task<ServiceResult<ICollection<JezykDto>>> GetJezyki()
    {
        return ServiceResult<ICollection<JezykDto>>.Ok(await jezykRepository.GetJezyki());
    }

    public async Task<ServiceResult<JezykDto?>> GetJezyk(int id)
    {
        try
        {
            return id < 1
                ? ServiceResult<JezykDto?>.BadRequest(new ErrorItem("Nieprawidłowe id jezyka: " + id))
                : ServiceResult<JezykDto?>.Ok(await jezykRepository.GetJezyk(id));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<JezykDto?>.NotFound(new ErrorItem(e.Message));
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