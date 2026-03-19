using System.ComponentModel.DataAnnotations.Schema;
using Squadra.Server.Modules.Platformy.Models;

namespace Squadra.Server.Modules.WspieraneGry.Models;

[Table(nameof(GraNaPlatformie))]
public class GraNaPlatformie
{
    public int IdWspieranejGry { get; set; }
    
    public int IdPlatformy { get; set; }
    
    [ForeignKey(nameof(IdWspieranejGry))]
    public virtual WspieranaGra WspieranaGra{ get; set; }
    
    [ForeignKey(nameof(IdPlatformy))]
    public virtual Platforma Platforma { get; set; }
}