namespace Squadra.Server.Modules.Profile.Models;

public class Jezyk
{
    public int Id { get; set; }
    public string Nazwa { get; set; } = null!;
    
    public virtual ICollection<JezykProfilu> JezykProfiluCollection { get; set; } = null!;
} 