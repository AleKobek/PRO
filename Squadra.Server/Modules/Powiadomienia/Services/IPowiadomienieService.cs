using System.Security.Claims;
using Squadra.Server.Modules.Powiadomienia.DTO;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Powiadomienia.Services;

public interface IPowiadomienieService
{
    public Task<ServiceResult<PowiadomienieDto>> GetPowiadomienie(int id, ClaimsPrincipal user);
    public Task<ServiceResult<ICollection<PowiadomienieDto>>> GetPowiadomieniaUzytkownika(int idUzytkownika);
    public Task<ServiceResult<bool>> CreatePowiadomienie(PowiadomienieCreateDto powiadomienie);
    public Task<ServiceResult<bool>> RozpatrzPowiadomienie(int id, bool? czyZaakceptowane, ClaimsPrincipal user);
    public Task<ServiceResult<bool>> WyslijZaproszenieDoZnajomychPoLoginie(int idZapraszajacego, string loginZaproszonego);
    public Task<ServiceResult<bool>> WyslijZaproszenieDoZnajomychPoId(int idZapraszajacego, int idZapraszanego);
}