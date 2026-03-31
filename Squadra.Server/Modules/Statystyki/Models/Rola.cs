using Squadra.Server.Modules.WspieraneGry.Models;

namespace Squadra.Server.Modules.Statystyki.Models;

public class Rola
{
    public int Id { get; set; }
    public string Nazwa { get; set; } = null!;
    public int IdGry { get; set; }
    
    public virtual WspieranaGra Gra { get; set; } = null!;
}