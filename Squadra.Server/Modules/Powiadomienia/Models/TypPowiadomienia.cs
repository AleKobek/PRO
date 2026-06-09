namespace Squadra.Server.Modules.Powiadomienia.Models;

public class TypPowiadomienia
{
    public int Id { get; set; }
    public string Nazwa { get; set; } = null!;
    
    public virtual ICollection<Powiadomienie> PowiadomienieCollection { get; set; } = null!;

}