using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squadra;

[Table(nameof(Status))]
public class Status
{
    [Key]
    public int Id { get; set; }
    
    [Required] [StringLength(10)]
    public string Nazwa { get; set; } = null!;
}