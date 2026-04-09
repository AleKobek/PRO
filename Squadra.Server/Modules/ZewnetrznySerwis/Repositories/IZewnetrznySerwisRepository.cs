using Squadra.Server.Modules.ZewnetrznySerwis.DTO;

namespace Squadra.Server.Modules.ZewnetrznySerwis.Repositories;

public interface IZewnetrznySerwisRepository
{
    public Task<ICollection<ZewnetrznaPlatformaUzytkownikaDTO>> GetPlatformyUzytkownika(int idNaZewnetrzymSerwisie);
    public Task<ICollection<ZewnetrznaStatystykaUzytkownikaDTO>> GetStatystykiUzytkownika(int idNaZewnetrzymSerwisie);
    public Task<ICollection<ZewnetrznaGraUzytkownikaDTO>> GetGryUzytkownika(int idNaZewnetrzymSerwisie);
    public Task<ICollection<ZewnetrznaGraUzytkownikaNaPlatformieDTO>> GetGryUzytkownikaNaPlatformie(int idNaZewnetrzymSerwisie);
}