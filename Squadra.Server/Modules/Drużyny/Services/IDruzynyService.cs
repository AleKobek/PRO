using Squadra.Server.Modules.Drużyny.DTO;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Drużyny.Services;

public interface IDruzynyService
{
    public Task<ServiceResult<DruzynaDoTabelkiDto>> GetDruzynaDoTabelki(int idDruzyny);
    public Task<ServiceResult<DruzynaSzczegolyDto>> PodajSzczegolyDruzyny(int idDruzyny);
    public Task<ServiceResult<ICollection<MiejsceWDruzynieSzczegolyDto>>> GetCzlonkowieDruzynyDoWyswietlenia(int idDruzyny);
    public Task<ServiceResult<DaneDoFormularzaDruzynyZeStatystykamiDto>> GetDaneDoFormularzaDruzynyZeStatystykami(int idGry, int idUzytkownika);
    public Task<ServiceResult<DaneDoFormularzaDruzynyBezStatystykDto>> GetDaneDoFormularzaDruzynyBezStatystyk(int idGry);
}