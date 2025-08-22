using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Praca_Inzynierska.Models;

[Table(nameof(Uzytkownik))]
public class Uzytkownik
{
    [Key]
    public int Id { get; set; }

    [MaxLength(20)] [Required] public string Login { get; set; } = null!;
    
    [MaxLength(30)] [Required]
    public string Pseudonim { get; set; } = null!;
    
    [MaxLength(20)] [Required] [EmailAddress]
    public string Email { get; set; } = null!;
    
    [MaxLength(30)] [Required]
    public string Haslo { get; set; } = null!;
    
    [Column("id_regionu")]
    public int RegionId { get; set; } 
    
    [ForeignKey(nameof(RegionId))]
    public virtual Region Region { get; set; } = null!;
    
    [MaxLength(15)]
    public string? NumerTelefonu { get; set; }
    
    public virtual ICollection<JezykUzytkownika> JezykUzytkownikaCollection { get; set; } = null!;
    
    public virtual Profil Profil { get; set; } = null!;
}