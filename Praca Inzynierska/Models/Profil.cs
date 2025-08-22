using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Praca_Inzynierska.Models;

[Table(nameof(Profil))]
public class Profil
{
    [Key]
    public int IdUzytkownika { get; set; }
    
    [MaxLength(10)]
    public string? Zaimki { get; set; }
    
    [MaxLength(100)]
    public string? Opis { get; set; }
    
    [ForeignKey(nameof(IdUzytkownika))]
    public virtual Uzytkownik Uzytkownik { get; set; } = null!;
}