using System.Security.Claims;
using Squadra.Server.Modules.Powiadomienia.DTO;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Powiadomienia.Services;

public interface IPowiadomienieService
{
    public Task<ServiceResult<PowiadomienieDto>> GetPowiadomienie(int id);
    public Task<ServiceResult<ICollection<PowiadomienieDto>>> GetPowiadomieniaUzytkownika(int idUzytkownika);
    public Task<ServiceResult<bool>> CzyUzytkownikMaPowiadomienieDanegoTypuPowiazaneZObiektami(int idUzytkownika, int idTypu, int idPowiazanegoObiektu, int? idDrugiegoPowiazanegoObiektu);
    public Task<ServiceResult<bool>> CreatePowiadomienie(PowiadomienieCreateDto powiadomienie);
    public Task<ServiceResult<bool>> DeletePowiadomieniaZwiazaneZUzytkownikiem(int idUzytkownika);
    public Task<ServiceResult<bool>> DeletePowiadomieniaDanegoTypuPowiazaneZObiektami(int? idUzytkownika, int idTypu, int idPowiazanegoObiektu, int? idDrugiegoPowiazanegoObiektu);
    public Task<ServiceResult<bool>> WyslijZaproszenieDoZnajomychPoLoginie(int idZapraszajacego, string loginZaproszonego);
    public Task<ServiceResult<bool>> WyslijZaproszenieDoZnajomychPoId(int idZapraszajacego, int idZapraszanego);
    public Task<ServiceResult<bool>> WyslijPowiadomienieODolaczeniuDoDruzyny(int idDolaczajacego, int idKapitana, int idDruzyny, string nazwaDruzyny, string? nazwaRoli);
    public Task<ServiceResult<bool>> WyslijZaproszenieNaMiejsceWDruzynie(int idZapraszanego, int idDruzyny, string nazwaDruzyny, int idMiejsca, string? nazwaRoli);
    public Task<ServiceResult<bool>> WyslijPowiadomienieOUsunieciuZDruzyny(int idUsuwanego, int idDruzyny, string nazwaDruzyny);
    public Task<ServiceResult<bool>> WyslijPowiadomienieOWyjsciuZDruzyny(int idKapitana, int? idOpuszczajacego, int idDruzyny, string nazwaDruzyny, string? nazwaRoli, bool czyPrzyUsuwaniuKonta);
    public Task<ServiceResult<bool>> WyslijPowiadomienieORozwiazaniuDruzyny(int idOdbiorcy, string nazwaDruzyny);
}