using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squadra;
[Table(nameof(Region))]
public class Region
{
    [Key]
    public int Id { get; set; }
    
    [Required] [StringLength(20)]
    public string Nazwa { get; set; } = null!;
    
    // region musi mieć kraj! nie będzie regionu "brak"
    [Column("id_kraju")]
    public int KrajId { get; set; }
    
    [ForeignKey(nameof(KrajId))]
    public virtual Kraj Kraj { get; set; } = null!;
    
    public virtual ICollection<Profil> ProfilCollection { get; set; } = null!;
}