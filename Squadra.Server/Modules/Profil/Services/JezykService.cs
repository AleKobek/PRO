using Squadra.Server.DTO.JezykStopien;
using Squadra.Server.Exceptions;
using Squadra.Server.Repositories;

namespace Squadra.Server.Services;

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
                ? ServiceResult<JezykDto?>.NotFound(new ErrorItem("Jezyk o id " + id + " nie istnieje"))
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
                ? ServiceResult<ICollection<JezykOrazStopienDto>>.NotFound(
                    new ErrorItem("Jezyk o id " + id + " nie istnieje"))
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
                ? ServiceResult<ICollection<JezykOrazStopienDto>>.NotFound(
                    new ErrorItem("Jezyk o id " + profilId + " nie istnieje"))
                : ServiceResult<ICollection<JezykOrazStopienDto>>.Ok(
                    await jezykRepository.ZmienJezykiProfilu(profilId, noweJezyki));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<ICollection<JezykOrazStopienDto>>.NotFound(new ErrorItem(e.Message));
        }
    }

}