using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Squadra.Server.Modules.WspieraneGry.Models;

namespace Squadra.Server.Modules.Platformy.Models;

[Table(nameof(Platforma))]
public class Platforma
{
    [Key]
    public int Id { get; set; }
    
    [Required] [StringLength(40)]
    public string Nazwa { get; set; } = null!;

    [Required] 
    public byte[] Logo { get; set; } = null!;
    
    public virtual ICollection<UzytkownikPlatforma> UzytkownikPlatformaCollection { get; set; } = null!;
    public virtual ICollection<GraNaPlatformie> GraNaPlatformieCollection { get; set; } = null!;
}