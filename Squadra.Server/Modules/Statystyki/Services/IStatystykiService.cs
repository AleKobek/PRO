using Squadra.Server.Modules.Drużyny.DTO;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Statystyki.DTO;
using Squadra.Server.Modules.Statystyki.Models;

namespace Squadra.Server.Modules.Statystyki.Services;

public interface IStatystykiService
{
    public Task<ServiceResult<StatystykaDTO>> GetStatystyka(int idStatystyki);
    public Task<ServiceResult<string>> GetGodzinyGrania(int idUzytkownika, int idGry);
    public Task<ServiceResult<ICollection<CzasRozgrywkiDTO>>> GetGodzinyGraniaUzytkownika(int idUzytkownika);
    public Task<ServiceResult<WartoscStatystykiDTO?>> GetWartoscStatystyki(int idUzytkownika, int idStatystyki);
    public Task<ServiceResult<ICollection<StatystykiDoTabelkiDTO>>> GetStatystykiUzytkownikaZGry(int idUzytkownika, int idGry);
    public Task<ServiceResult<bool>> UpdateStatystykiUzytkownika(int idUzytkownika, List<StatystykaUzytkownika> noweStatystyki);
    public Task<ServiceResult<bool>> UsunStatystykiUzytkownika(int idUzytkownika);
    public ServiceResult<bool> CzySpelniaWymagania(ICollection<WartoscStatystykiDTO> wymagania, ICollection<WartoscStatystykiDTO> statystykiDoSprawdzenia);
    public Task<ServiceResult<ICollection<WymaganieDruzynyDoWyswietleniaDto>>> GetWymaganiaDruzynyDoWyswietlenia(int idDruzyny);
    public Task<ServiceResult<ICollection<WartoscStatystykiDTO>>> GetWymaganiaDruzyny(int idDruzyny);
    public Task<ServiceResult<ICollection<RolaDto>>> GetRoleGry(int idGry);
    public Task<ServiceResult<StatystykiDoFormularzaDto>> GetStatystykiDoFormularza(int idGry, int idUzytkownika);
    public Task<ServiceResult<ICollection<RolaDto>>> GetRole();
    public Task<ServiceResult<ICollection<RangiStatystykiDto>>> GetRangiGry(int idGry);
    public Task<ServiceResult<RolaDto>> GetRola(int idRoli);
    public ServiceResult<ICollection<int>> FiltrujNieistniejaceStatystyki(ICollection<int> idStatystyk);
}