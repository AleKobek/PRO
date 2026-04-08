using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Statystyki.DTO;
using Squadra.Server.Modules.Statystyki.Models;

namespace Squadra.Server.Modules.Statystyki.Services;

public interface IStatystykiService
{
    public Task<ServiceResult<string>> GetGodzinyGrania(int idUzytkownika, int idGry);
    public Task<ServiceResult<ICollection<CzasRozgrywkiDTO>>> GetGodzinyGraniaUzytkownika(int idUzytkownika);
    public Task<ServiceResult<string?>> GetWartoscStatystyki(int idUzytkownika, int idStatystyki);
    public Task<ServiceResult<ICollection<StatystykaDTO>>> GetStatystykiZGry(int idUzytkownika, int idGry);
    public Task<ServiceResult<ICollection<StatystykaUzytkownika>>> UpdateStatystykiUzytkownika(int idUzytkownika);
    public Task<ServiceResult<bool>> UsunStatystykiUzytkownika(int idUzytkownika);
}