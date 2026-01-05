using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squadra.Server.Models;

[Table(nameof(Wiadomosc))]
public class TypWiadomosci
{
    public int Id { get; set; }
    
    [Required] [StringLength(50)]
    public string Nazwa { get; set; } = null!;

    public virtual ICollection<Wiadomosc> WiadomosciCollection { get; set; } = null!;
}