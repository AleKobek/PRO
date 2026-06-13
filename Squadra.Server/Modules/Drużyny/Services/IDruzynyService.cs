using Squadra.Server.Modules.Drużyny.DTO;
using Squadra.Server.Modules.Platformy;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Drużyny.Services;

public interface IDruzynyService
{
    public Task<ServiceResult<DruzynaDto>> GetDruzyna(int id);
    public Task<ServiceResult<DruzynaDto>> GetDruzynaMiejsca(int idMiejsca);
    public Task<ServiceResult<MiejsceWDruzynieDto>> GetMiejsceWDruzynie(int idMiejscaWDruzynie);
    public Task<ServiceResult<TabelkaDruzynResDto>> GetWszystkieDruzynyUzytkownikaDoTabelki(int idUzytkownika);
    public Task<ServiceResult<ICollection<DruzynaDoTabelkiDto>>> GetDruzynyDoTabelki(int[] idDruzyn);
    public Task<ServiceResult<DruzynaSzczegolyDto>> PodajSzczegolyDruzyny(int idDruzyny, int idUzytkownika);
    public Task<ServiceResult<ICollection<MiejsceWDruzynieSzczegolyDto>>> GetCzlonkowieDruzynyDoWyswietlenia(int idDruzyny);
    public Task<ServiceResult<DaneDoFormularzaDruzynyZeStatystykamiDto>> GetDaneDoFormularzaDruzynyZeStatystykami(int idGry, int idUzytkownika);
    public Task<ServiceResult<DaneDoFormularzaDruzynyBezStatystykDto>> GetDaneDoFormularzaDruzynyBezStatystyk(int idGry);
    public Task<ServiceResult<DaneDoFormularzaWyszukiwaniaDruzyny>> GetDaneDoFormularzaWyszukiwaniaDruzyny(int idUzytkownika);
    public Task<ServiceResult<ICollection<NastrojRozgrywkiDto>>> GetNastrojeRozgrywki();
    public Task<ServiceResult<bool>> CzyUzytkownikSpelniaWymaganieMiejsca(int idMiejsca, int idUzytkownika);
    public Task<ServiceResult<bool>> CzyUzytkownikSpelniaWymaganiaDruzyny(int idDruzyny, int idUzytkownika);
    public Task<ServiceResult<bool>> CzyUzytkownikPrzekraczaMaksLiczbeDruzyn(int idGry, int idUzytkownika);
    public Task<ServiceResult<bool>> CzyUzytkownikNalezyDoDruzyny(int idUzytkownika, int idDruzyny);
    public Task<ServiceResult<bool>> StworzDruzyne(CreateDruzynaReqDto druzynaReq, int idKapitana);
    public Task<ServiceResult<bool>> UsunDruzyne(int idDruzyny, int idUsuwajacegoUzytkownika);
    public Task<ServiceResult<bool>> OpuscDruzyne(int idDruzyny, int idUzytkownika, bool czyPrzyUsuwaniuKonta);
    public Task<ServiceResult<bool>> OproznijMiejsceWDruzynie(int idMiejsca, int idUsuwajacegoUzytkownika);
    public Task<ServiceResult<bool>> PrzerwijIntegracjeUzytkownikaOdnosnieDruzyn(int idUzytkownika);
    public Task<ServiceResult<bool>> UsunWszystkieDruzynyDlaUzytkownika(int idUzytkownika);
    public Task<ServiceResult<bool>> UpdateDruzyna(int idDruzyny, int idUzytkownika, DruzynaUpdateDto druzynaReq);
    public Task<ServiceResult<TabelkaDruzynResDto>> WyszukajDruzyny(WyszukajDruzyneReqDto req, int idUzytkownika);
    public Task<ServiceResult<bool>> DodajUzytkownikaNaMiejsce(int idMiejsca, int idUzytkownika);
    public Task<ServiceResult<bool>> ZaprosUzytkownikaNaMiejsce(int idMiejsca, int idZapraszanegoUzytkownika, int idZapraszajacegoUzytkownika);
    public Task<ServiceResult<bool>> ZaprosUzytkownikaNaMiejscePoLoginie(int idMiejsca, string login, int idZapraszajacegoUzytkownika);
}