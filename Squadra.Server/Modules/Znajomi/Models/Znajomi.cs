using System.ComponentModel.DataAnnotations.Schema;

namespace Squadra.Server.Models;

[Table(nameof(Znajomi))]
public class Znajomi
{
    [Column("id_znajomego_1")]
    public int IdUzytkownika1 { get; set; }
    [Column("id_znajomego_2")]
    public int IdUzytkownika2 { get; set; }
    
    public DateOnly DataNawiazaniaZnajomosci { get; set; }
    
    public DateTime? OstatnieOtwarcieCzatuUzytkownika1 { get; set; }
    public DateTime? OstatnieOtwarcieCzatuUzytkownika2 { get; set; }
    
    
    [ForeignKey(nameof(IdUzytkownika1))]
    public virtual Uzytkownik Uzytkownik1 { get; set; } = null!;
    
    [ForeignKey(nameof(IdUzytkownika2))]
    public virtual Uzytkownik Uzytkownik2 { get; set; } = null!;
}