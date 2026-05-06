using Squadra.Server.Modules.Drużyny.Models;
using Squadra.Server.Modules.Statystyki.Models;

namespace Squadra.Server.Modules.Drużyny.Repositories;

public interface IDruzynyRepository
{
    public Task<Druzyna> GetDruzyna(int idDruzyny);
    public Task<ICollection<MiejsceWDruzynie>> GetMiejscaWDruzynie(int idDruzyny);
    public Task<ICollection<NastrojRozgrywki>> GetNastrojeRozgrywki();
    public Task<NastrojRozgrywki> GetNastrojRozgrywki(int idNastroju);
}