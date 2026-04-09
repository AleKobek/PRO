using Squadra.Server.Modules.ZewnetrznaPlatforma.DTO;

namespace Squadra.Server.Modules.ZewnetrznaPlatforma.Repositories;

public interface IZewnetrznaPlatformaRepository
{
    public Task<ICollection<ZewnetrznaPlatformaUzytkownikaDTO>> GetPlatformyUzytkownika(int idNaZewnetrzymSerwisie);
    public Task<ICollection<ZewnetrznaStatystykaUzytkownikaDTO>> GetStatystykiUzytkownika(int idNaZewnetrzymSerwisie);
    public Task<ICollection<ZewnetrznaGraUzytkownikaDTO>> GetGryUzytkownika(int idNaZewnetrzymSerwisie);
    public Task<ICollection<ZewnetrznaGraUzytkownikaNaPlatformieDTO>> GetGryUzytkownikaNaPlatformie(int idNaZewnetrzymSerwisie);
}