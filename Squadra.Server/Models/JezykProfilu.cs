using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Squadra.Server.Models;

[Table(nameof(JezykProfilu))]
[PrimaryKey(nameof(UzytkownikId), nameof(JezykId))]
public class JezykProfilu
{
 
    [Column("id_uzytkownika")]
    public int UzytkownikId { get; set; }

    [Column("id_jezyka")]
    public int JezykId { get; set; }
    
    [Column("id_stopnia_bieglosci")]
    public int StopienBieglosciId { get; set; }
    
    [ForeignKey(nameof(UzytkownikId))]
    public virtual Profil Profil { get; set; } = null!;
    
    [ForeignKey(nameof(JezykId))]
    public virtual Jezyk Jezyk { get; set; } = null!;
    
    [ForeignKey(nameof(StopienBieglosciId))]
    public virtual StopienBieglosciJezyka StopienBieglosciJezyka { get; set; } = null!;
}