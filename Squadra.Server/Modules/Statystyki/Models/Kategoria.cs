using Squadra.Server.Modules.WspieraneGry.Models;

namespace Squadra.Server.Modules.Statystyki.Models;

public class Kategoria
{ 
    public int Id { get; set; }
    public string Nazwa { get; set; } = null!;
    public int IdGry { get; set; }
    public bool CzyToCzasRozgrywki { get; set; }
    
    public virtual WspieranaGra Gra { get; set; } = null!;
    public virtual ICollection<Statystyka> StatystykaCollection { get; set; } = null!;
}