using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Squadra.Server.Modules.Uzytkownicy.Models;

namespace Squadra.Server.Modules.Platformy.Models;

[Table(nameof(UzytkownikPlatforma))]
[PrimaryKey(nameof(UzytkownikId), nameof(PlatformaId))]
public class UzytkownikPlatforma
{
    public int UzytkownikId { get; set; }
    public int PlatformaId { get; set; }
    
    [Required] [StringLength(40)]
    public string PseudonimNaPlatformie { get; set; } = null!;
    
    [ForeignKey(nameof(PlatformaId))]
    public virtual Platforma Platforma { get; set; }
    
    [ForeignKey(nameof(UzytkownikId))]
    public virtual Uzytkownik Uzytkownik { get; set; }
}