using Squadra.Server.Modules.Powiadomienia.DTO;
using Squadra.Server.Modules.Powiadomienia.Models;

namespace Squadra.Server.Modules.Powiadomienia.Repositories;

public interface IPowiadomienieRepository
{
    public Task<Powiadomienie> GetPowiadomienie(int id);
    public Task<ICollection<Powiadomienie>> GetPowiadomieniaUzytkownika(int idUzytkownika);
    public Task<ICollection<Powiadomienie>> GetZaproszeniaDoDruzynyUzytkownika(int idUzytkownika);
    public Task<ICollection<Powiadomienie>> PodajPowiadomieniaUzytkownikaPrzekraczajaceLimit(int idUzytkownika);
    public Task<bool> CzyUzytkownikMaPowiadomienieDanegoTypuPowiazaneZObiektami(int idUzytkownika, int idTypu, int idPowiazanegoObiektu, int? idDrugiegoPowiazanegoObiektu);
    public Task<bool> CreatePowiadomienie(PowiadomienieCreateDto powiadomienie);
    public Task<bool> DeletePowiadomienie(int id);
    public Task<bool> UsunPowiadomienia(ICollection<Powiadomienie> powiadomienia);
    public Task<bool> DeletePowiadomieniaZwiazaneZUzytkownikiem(int idUzytkownika);
    public Task<bool> UsunPowiadomieniaZwiazaneZDruzyna(int idDruzyny);
    public Task<bool> DeletePowiadomieniaDanegoTypuPowiazaneZObiektami(int? idUzytkownika, int idTypu, int idPowiazanegoObiektu, int? idDrugiegoPowiazanegoObiektu);
    public Task<string> GetNazwaTypuPowiadomienia(int idTypuPowiadomienia);
}