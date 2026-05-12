using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Platformy.DTO;
using Squadra.Server.Modules.Platformy.Models;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.WspieraneGry.DTO;
using Squadra.Server.Modules.WspieraneGry.Repositories;

namespace Squadra.Server.Modules.WspieraneGry.Services;

public class WspieranaGraService(IWspieranaGraRepository wspieranaGraRepository) : IWspieranaGraService
{
    public async Task<ServiceResult<ICollection<WspieranaGraDto>>> GetWspieraneGry()
    {
        var gry = await wspieranaGraRepository.GetWspieraneGry();
        return ServiceResult<ICollection<WspieranaGraDto>>.Ok(gry.Select(g => new WspieranaGraDto(g.Id, g.Tytul, g.Wydawca, g.Gatunek)).ToList());
    }

    public async Task<ServiceResult<WspieranaGraDto>> GetWspieranaGra(int idGry)
    {
        try
        {
            if (idGry <= 0)
                return ServiceResult<WspieranaGraDto>.BadRequest(new ErrorItem("Nieprawidłowe id gry: " + idGry));
            var gra = await wspieranaGraRepository.GetWspieranaGra(idGry);
            return ServiceResult<WspieranaGraDto>.Ok(new WspieranaGraDto(gra.Id, gra.Tytul, gra.Wydawca, gra.Gatunek));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<WspieranaGraDto>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<ICollection<MinInfoWspieranaGraDTO>>> GetWspieraneGryMinInfo()
    {
        var gry = await wspieranaGraRepository.GetWspieraneGry();
        return ServiceResult<ICollection<MinInfoWspieranaGraDTO>>.Ok(gry.Select(x => new MinInfoWspieranaGraDTO(x.Id, x.Tytul)).ToList());
    }
    
    public async Task<ServiceResult<ICollection<GraZPlatformaDTO>>> GetWspieraneGryZPlatformami()
    {
        return ServiceResult<ICollection<GraZPlatformaDTO>>.Ok(await wspieranaGraRepository.GetWspieraneGryZPlatformami());
    }
    
    public async Task<ServiceResult<ICollection<PlatformaDto>>> GetPlatformyGry(int idGry)
    {
        try
        {
            if (idGry <= 0)
                return ServiceResult<ICollection<PlatformaDto>>.BadRequest(new ErrorItem("Nieprawidłowe id gry: " + idGry));
            var platformyRes = await wspieranaGraRepository.GetPlatformyGry(idGry);
            return ServiceResult<ICollection<PlatformaDto>>.Ok(platformyRes.Select(x => new PlatformaDto(x.Id, x.Nazwa, x.Logo)).ToList());
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<ICollection<PlatformaDto>>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<ICollection<Platforma>>> GetPlatformyGryUzytkownika(int idGry, int idUzytkownika)
    {
        try
        {
            if (idGry <= 0)
                return ServiceResult<ICollection<Platforma>>.BadRequest(new ErrorItem("Nieprawidłowe id gry: " + idGry));
            if (idUzytkownika <= 0)
                return ServiceResult<ICollection<Platforma>>.BadRequest(new ErrorItem("Nieprawidłowe id użytkownika: " + idUzytkownika));
            return ServiceResult<ICollection<Platforma>>.Ok(await wspieranaGraRepository.GetPlatformyGryUzytkownika(idGry, idUzytkownika));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<ICollection<Platforma>>.NotFound(new ErrorItem(e.Message));
        }
    }
}