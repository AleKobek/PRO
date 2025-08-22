using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Praca_Inzynierska.Models;

[Table(nameof(Jezyk))]
public class Jezyk
{
    [Key]
    public int Id { get; set; }
    
    [Required] [StringLength(20)]
    public string Nazwa { get; set; } = null!;
    
    public virtual ICollection<JezykUzytkownika> JezykUzytkownikaCollection { get; set; } = null!;
} 