namespace Squadra.Server.Modules.Profile.Models;

public class Kraj
{
    public int Id { get; set; }
    public string Nazwa { get; set; } = null!;
    
    public virtual ICollection<Region> RegionCollection { get; set; } = null!;
}