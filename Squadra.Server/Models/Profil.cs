using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squadra;

[Table(nameof(Profil))]
public class Profil
{
    [Key]
    public int IdUzytkownika { get; set; }
    
    [MaxLength(10)]
    public string? Zaimki { get; set; }
    
    [MaxLength(30)] [Required]
    public string Pseudonim { get; set; } = null!;
    
    [MaxLength(100)]
    public string? Opis { get; set; }
    
    [Column("id_regionu")]
    public int RegionId { get; set; } 
    
    public byte[]? Awatar { get; set; }
    
    [ForeignKey(nameof(IdUzytkownika))]
    public virtual Uzytkownik Uzytkownik { get; set; } = null!;
    
    [ForeignKey(nameof(RegionId))]
    public virtual Region Region { get; set; } = null!;
    
    public virtual ICollection<JezykProfilu> JezykUzytkownikaCollection { get; set; } = null!;
    
}