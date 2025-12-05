using System.Security.Claims;
using Squadra.Server.DTO.Powiadomienie;

namespace Squadra.Server.Services;

public interface IPowiadomienieService
{
    public Task<ServiceResult<PowiadomienieDto>> GetPowiadomienie(int id, ClaimsPrincipal user);
    public Task<ServiceResult<ICollection<PowiadomienieDto>>> GetPowiadomieniaUzytkownika(int idUzytkownika);
    public Task<ServiceResult<bool>> CreatePowiadomienie(PowiadomienieCreateDto powiadomienie);
    public Task<ServiceResult<bool>> RozpatrzPowiadomienie(int id, bool? czyZaakceptowane, ClaimsPrincipal user);
    public Task<ServiceResult<bool>> WyslijZaproszenieDoZnajomych(PowiadomienieCreateDto dto, string login);
}