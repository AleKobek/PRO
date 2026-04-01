using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Statystyki.DTO;

namespace Squadra.Server.Modules.Statystyki.Services;

public interface IStatystykiService
{
    public Task<ServiceResult<string>> GetGodzinyGrania(int idUzytkownika, int idGry);
    public Task<ServiceResult<string?>> GetWartoscStatystyki(int idUzytkownika, int idStatystyki);
    public Task<ServiceResult<ICollection<StatystykaDTO>>> GetStatystykiZGry(int idUzytkownika, int idGry);
}