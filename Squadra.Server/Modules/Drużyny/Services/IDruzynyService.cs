using Squadra.Server.Modules.Drużyny.DTO;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Drużyny.Services;

public interface IDruzynyService
{
    public Task<ServiceResult<ICollection<DruzynaDoTabelkiDto>>> GetWszystkieDruzynyUzytkownikaDoTabelki(int idUzytkownika);
    public Task<ServiceResult<DruzynaSzczegolyDto>> PodajSzczegolyDruzyny(int idDruzyny);
    public Task<ServiceResult<ICollection<MiejsceWDruzynieSzczegolyDto>>> GetCzlonkowieDruzynyDoWyswietlenia(int idDruzyny);
    public Task<ServiceResult<DaneDoFormularzaDruzynyZeStatystykamiDto>> GetDaneDoFormularzaDruzynyZeStatystykami(int idGry, int idUzytkownika);
    public Task<ServiceResult<DaneDoFormularzaDruzynyBezStatystykDto>> GetDaneDoFormularzaDruzynyBezStatystyk(int idGry);
    public Task<ServiceResult<DaneDoFormularzaWyszukiwaniaDruzyny>> GetDaneDoFormularzaWyszukiwaniaDruzyny(int idUzytkownika);
    public Task<ServiceResult<bool>> StworzDruzyne(CreateDruzynaReqDto druzynaReq, int idKapitana);
    public Task<ServiceResult<bool>> UsunDruzyne(int idDruzyny, int idUsuwajacegoUzytkownika);
    public Task<ServiceResult<bool>> OpuśćDruzyne(int idDruzyny, int idUzytkownika);
    public Task<ServiceResult<bool>> WyrzucUzytkownikaZeWszystkichDruzyn(int idUzytkownika);
}