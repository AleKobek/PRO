using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Modules.Platformy.DTO;

namespace Squadra.Server.Modules.Platformy.Repositories;

public class UzytkownikPlatformaRepository(AppDbContext context, IPlatformaRepository platformaRepository) : IUzytkownikPlatformaRepository
{
    public async Task<ICollection<PlatformaUzytkownikaDTO>> GetPlatformyUzytkownika(int idUzytkownika)
    {
        var platformyUzytkownika = await context.UzytkownikPlatforma.Where(up => up.UzytkownikId == idUzytkownika).ToListAsync();
        var platformy = new List<PlatformaUzytkownikaDTO>();
        foreach (var up in platformyUzytkownika)
        {
            var platforma = await platformaRepository.GetPlatformaById(up.PlatformaId);
            if (platforma != null)
            {
                platformy.Add(new PlatformaUzytkownikaDTO(
                    up.PlatformaId,
                    platforma.Nazwa,
                    platforma.Logo,
                    up.PseudonimNaPlatformie
                ));
            }
        }
        return platformy;
    }
}