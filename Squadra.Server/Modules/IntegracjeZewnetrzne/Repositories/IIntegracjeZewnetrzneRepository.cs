using Squadra.Server.Modules.IntegracjeZewnetrzne.DTO;

namespace Squadra.Server.Modules.IntegracjeZewnetrzne.Repositories;

public interface IIntegracjeZewnetrzneRepository
{
    public Task<int?> SprawdzDaneLogowania(string login, string hasloHash);
    public Task<ICollection<ZewnetrznaPlatformaUzytkownikaDTO>> GetPlatformyUzytkownika(int idNaZewnetrzymSerwisie);
    public Task<ICollection<ZewnetrznaStatystykaUzytkownikaDTO>> GetStatystykiUzytkownika(int idNaZewnetrzymSerwisie);
    public Task<ICollection<ZewnetrznaGraUzytkownikaDTO>> GetGryUzytkownika(int idNaZewnetrzymSerwisie);
    public Task<ICollection<ZewnetrznaGraUzytkownikaNaPlatformieDTO>> GetGryUzytkownikaNaPlatformie(int idNaZewnetrzymSerwisie);
}