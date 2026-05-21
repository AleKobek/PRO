using Squadra.Server.Modules.Drużyny.DTO;
using Squadra.Server.Modules.Drużyny.Models;

namespace Squadra.Server.Modules.Drużyny.Repositories;

public interface IDruzynyRepository
{
    public Task<Druzyna> GetDruzyna(int idDruzyny);
    public Task<ICollection<Druzyna>> GetDruzynyUzytkownika(int idUzytkownika);
    public Task<ICollection<MiejsceWDruzynie>> GetMiejscaWDruzynie(int idDruzyny);
    public Task<ICollection<NastrojRozgrywki>> GetNastrojeRozgrywki();
    public Task<NastrojRozgrywki> GetNastrojRozgrywki(int idNastroju);
    public Task<bool> StworzDruzyne(CreateDruzynaReqDto druzynaReq, int idKapitana);
    public Task<bool> UsunDruzyne(int idDruzyny);
    public Task<bool> OpuscDruzyne(int idDruzyny, int idUzytkownika);
    public Task<bool> WyrzucUzytkownikaZeWszystkichDruzyn(int idUzytkownika);
    public Task<bool> UsunWszystkieDruzynyUzytkownika(int idUzytkownika);
}