namespace Squadra.Server.Modules.Drużyny.Models;

public class NastrojRozgrywki
{
    public int Id { get; set; }
    public string Nazwa { get; set; } = null!;
    
    public virtual ICollection<Druzyna> DruzynaCollection { get; set; } = new List<Druzyna>();
}