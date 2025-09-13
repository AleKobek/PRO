using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squadra;

[Table(nameof(Jezyk))]
public class Jezyk
{
    [Key]
    public int JezykId { get; set; }
    
    [Required] [StringLength(20)]
    public string Nazwa { get; set; } = null!;
    
    public virtual ICollection<JezykProfilu> JezykProfiluCollection { get; set; } = null!;
} 