using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squadra.Server.Models;

[Table(nameof(Kraj))]
public class Kraj
{
    [Key]
    public int Id { get; set; }

    [Required] [StringLength(20)] 
    public string Nazwa { get; set; } = null!;
    
    public virtual ICollection<Region> RegionCollection { get; set; } = null!;
}