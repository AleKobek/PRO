using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Statystyki.DTO;
using Squadra.Server.Modules.Statystyki.Models;

namespace Squadra.Server.Modules.Statystyki.Services;

public interface IStatystykiService
{
    public Task<ServiceResult<Statystyka>> GetStatystyka(int idStatystyki);
    public Task<ServiceResult<string>> GetGodzinyGrania(int idUzytkownika, int idGry);
    public Task<ServiceResult<ICollection<CzasRozgrywkiDTO>>> GetGodzinyGraniaUzytkownika(int idUzytkownika);
    public Task<ServiceResult<WartoscStatystykiDTO?>> GetWartoscStatystyki(int idUzytkownika, int idStatystyki);
    public Task<ServiceResult<ICollection<StatystykiDoTabelkiDTO>>> GetStatystykiZGry(int idUzytkownika, int idGry);
    public Task<ServiceResult<bool>> UpdateStatystykiUzytkownika(int idUzytkownika, List<StatystykaUzytkownika> noweStatystyki);
    public Task<ServiceResult<bool>> UsunStatystykiUzytkownika(int idUzytkownika);
    public ServiceResult<bool> CzySpelniaWymagania(ICollection<WartoscStatystykiDTO> wymagania, ICollection<WartoscStatystykiDTO> statystykiDoSprawdzenia);
    public Task<ServiceResult<ICollection<WymaganieDruzynyDoWyswietleniaDto>>> GetWymaganiaDruzynyDoWyswietlenia(int idDruzyny);
    public Task<ServiceResult<StatystykiDoFormularzaDto>> GetStatystykiDoFormularza(int idGry);
}