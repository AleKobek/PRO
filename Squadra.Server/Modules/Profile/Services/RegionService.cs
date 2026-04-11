using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Profile.DTO.KrajRegion;
using Squadra.Server.Modules.Profile.Repositories;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Profile.Services;

public class RegionService(IRegionRepository regionRepository) : IRegionService
{
    public async Task<ServiceResult<ICollection<RegionDto>>> GetRegiony()
    {
        return ServiceResult<ICollection<RegionDto>>.Ok(await regionRepository.GetRegiony());
    }

    public async Task<ServiceResult<RegionDto>> GetRegion(int id)
    {
        try
        {
            if (id < 1) return ServiceResult<RegionDto>.BadRequest(new ErrorItem("Nieprawidłowe id regionu: " + id));
            var region = await regionRepository.GetRegion(id);
            if (region == null) return ServiceResult<RegionDto>.NotFound(new ErrorItem("Region o id " + id + " nie istnieje"));
            return ServiceResult<RegionDto>.Ok(region);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<RegionDto>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<RegionKrajDto>> GetRegionIKraj(int id)
    {
        try
        {
            return id < 1
                ? ServiceResult<RegionKrajDto>.BadRequest(new ErrorItem("Nieprawidłowe id regionu: " + id)) 
                // w Ok() value nie może być nullem, dlatego robimy sami
                : new ServiceResult<RegionKrajDto> {Succeeded = true, StatusCode = 200, Value = await regionRepository.GetRegionIKraj(id)};
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<RegionKrajDto>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<ICollection<RegionDto>>> GetRegionyKraju(int krajId)
    {
        try{
            
            return ServiceResult<ICollection<RegionDto>>.Ok(await regionRepository.GetRegionyKraju(krajId));
            
        }catch(NieZnalezionoWBazieException e){
            
            return ServiceResult<ICollection<RegionDto>>.NotFound(new ErrorItem(e.Message));
            
        }
    }
}