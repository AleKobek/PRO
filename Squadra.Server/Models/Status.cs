using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squadra.Server.Models;

[Table(nameof(Status))]
public class Status
{
    [Key]
    public int Id { get; set; }
    
    [Required] [StringLength(20)]
    public string Nazwa { get; set; } = null!;
    
    public virtual ICollection<Uzytkownik> UzytkownikCollection { get; set; } = null!;
}