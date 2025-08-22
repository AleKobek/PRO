using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Praca_Inzynierska.Models;

[Table(nameof(JezykUzytkownika))]
[PrimaryKey(nameof(UzytkownikId), nameof(JezykId))]
public class JezykUzytkownika
{
 
    [Column("id_uzytkownika")]
    public int UzytkownikId { get; set; }

    [Column("id_jezyka")]
    public int JezykId { get; set; }
    
    [Column("id_stopnia_bieglosci")]
    public int StopienBieglosciId { get; set; }
    
    [ForeignKey(nameof(UzytkownikId))]
    public virtual Uzytkownik Uzytkownik { get; set; } = null!;
    
    [ForeignKey(nameof(JezykId))]
    public virtual Jezyk Jezyk { get; set; } = null!;
    
    [ForeignKey(nameof(StopienBieglosciId))]
    public virtual StopienBieglosciJezyka StopienBieglosciJezyka { get; set; } = null!;
}