using Squadra.Server.Modules.Drużyny.DTO;
using Squadra.Server.Modules.Drużyny.Models;
using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Statystyki.Models;

namespace Squadra.Server.Modules.Drużyny.Repositories;

public interface IDruzynyRepository
{
    public Task<Druzyna> GetDruzyna(int idDruzyny);
    public Task<Druzyna> GetDruzynaMiejsca(int idMiejsca);
    public Task<ICollection<Druzyna>> GetDruzynyUzytkownika(int idUzytkownika);
    public Task<ICollection<Druzyna>> GetDruzyny(int[] idDruzyn);
    public Task<ICollection<MiejsceWDruzynie>> GetMiejscaWDruzynie(int idDruzyny);
    public Task<ICollection<MiejsceWDruzynie>> GetMiejscaWDruzynieUzytkownika(int idUzytkownika);
    public Task<int> GetIdMiejscaKapitanaDruzyny(int idDruzyny);
    public Task<int> GetNumerMiejsca(int idMiejsca);
    public Task<MiejsceWDruzynie> GetMiejsceWDruzynie(int idMiejsca);
    public Task<Rola?> GetRolaMiejscaWDruzynieUzytkownika(int idDruzyny, int idUzytkownika);
    public Task<int> GetIdKapitanaDruzynyMiejsca(int idMiejsca);
    public Task<ICollection<NastrojRozgrywki>> GetNastrojeRozgrywki();
    public Task<NastrojRozgrywki> GetNastrojRozgrywki(int idNastroju);
    public Task<DateTime?> GetDataOstatniegoOtwarciaCzatuUzytkownika(int idUzytkownika, int idDruzyny);
    public Task<bool> CzyUzytkownikSpelniaWymaganieMiejsca(int idMiejsca, int idUzytkownika);
    public Task<bool> CzyUzytkownikSpelniaWymaganeStatystykiDruzyny(int idDruzyny, int idUzytkownika);
    public  Task<bool> CzyUzytkownikNalezyDoDruzyny(int idUzytkownika, int idDruzyny);
    public Task<int?> StworzDruzyne(CreateDruzynaReqDto druzynaReq, int idKapitana);
    public Task<bool> UsunDruzyne(int idDruzyny);
    public Task<bool> DeleteMiejscaWDruzynie(int idDruzyny);
    public Task<bool> OpuscDruzyne(int idDruzyny, int idUzytkownika);
    public Task<bool> OproznijMiejsceWDruzynie(int idMiejsca);
    public Task<bool> WyrzucUzytkownikaZeWszystkichDruzyn(int idUzytkownika);
    public Task<bool> WyrzucUzytkownikaZeWszystkichZintegrowanychDruzyn(int idUzytkownika);
    public Task<bool> UsunWszystkieDruzynyUzytkownika(int idUzytkownika);
    public Task<bool> UsunWszystkieZintegrowaneDruzynyUzytkownika(int idUzytkownika);
    public Task<bool> UpdateDruzyna(int idDruzyny, DruzynaUpdateDto druzynaReq);
    public Task<ICollection<int>> WyszukajIdDruzyn(WyszukajDruzyneReqDto req, int idUzytkownika, ICollection<JezykOrazStopienDto> jezykiUzytkownika);
    public Task<bool> CzyUzytkownikOsiagnalMaksLiczbeDruzyn(int idUzytkownika, int idGry);
    public Task<bool> DodajUzytkownikaNaMiejsce(int idMiejsca, int idUzytkownika);
    public Task<bool> UpdateDataOstatniegoOtwarciaCzatu(int idDruzyny, int idUzytkownika);
}