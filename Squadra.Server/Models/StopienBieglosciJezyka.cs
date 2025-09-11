using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squadra;

[Table(nameof(StopienBieglosciJezyka))]
public class StopienBieglosciJezyka
{
    public int Id { get; set; }
    
    [Required] [StringLength(20)]
    public string Nazwa { get; set; } = null!;
    
    public int Wartosc { get; set; }
    
    public virtual ICollection<JezykProfilu> JezykUzytkownikaCollection { get; set; } = null!;
 
}