using Squadra.Server.Modules.Platformy.Models;

namespace Squadra.Server.Modules.WspieraneGry.Models;

public class GraNaPlatformie
{
    public int IdWspieranejGry { get; set; }
    public int IdPlatformy { get; set; }
    
    public virtual WspieranaGra WspieranaGra{ get; set; }
    public virtual Platforma Platforma { get; set; }
}