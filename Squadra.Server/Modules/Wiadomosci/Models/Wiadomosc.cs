using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squadra.Server.Models;

[Table(nameof(Wiadomosc))]
public class Wiadomosc
{

    [Key]
    public int Id { get; set; }
    
    [Column("id_nadawcy")]
    public int IdNadawcy { get; set; }
    [Column("id_odbiorcy")]
    public int IdOdbiorcy { get; set; }
    
    public DateTime DataWyslania { get; set; }
    
    [Required] [StringLength(1000)]
    public string Tresc { get; set; } = null!;
    
    public int IdTypuWiadomosci { get; set; }
    
    [ForeignKey(nameof(IdNadawcy))]
    public virtual Uzytkownik Nadawca { get; set; } = null!;
    
    [ForeignKey(nameof(IdOdbiorcy))]
    public virtual Uzytkownik Odbiorca { get; set; } = null!;

    [ForeignKey(nameof(IdTypuWiadomosci))]
    public virtual TypWiadomosci TypWiadomosci { get; set; } = null!;

}