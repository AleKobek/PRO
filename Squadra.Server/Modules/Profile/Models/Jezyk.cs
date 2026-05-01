using Squadra.Server.Modules.Drużyny.Models;

namespace Squadra.Server.Modules.Profile.Models;

public class Jezyk
{
    public int Id { get; set; }
    public string Nazwa { get; set; } = null!;
    
    public virtual ICollection<JezykProfilu> JezykProfiluCollection { get; set; } = null!;
    public virtual ICollection<Druzyna> DruzynaCollection { get; set; } = null!;
} 