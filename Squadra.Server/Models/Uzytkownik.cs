using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squadra.Server.Models;

[Table(nameof(Uzytkownik))]
public class Uzytkownik
{
    [Key]
    public int Id { get; set; }

    [MaxLength(20)] [Required] public string Login { get; set; } = null!;
    
   
    
    [MaxLength(20)] [Required] [EmailAddress]
    public string Email { get; set; } = null!;
    
    [MaxLength(30)] [Required]
    public string Haslo { get; set; } = null!;
    
    [MaxLength(15)]
    public string? NumerTelefonu { get; set; }
    
    // te dwa nie są mi potrzebne do prototypu, ale Ola z przyszłości mi za to podziękuje!
    
    public DateOnly? DataUrodzenia { get; set; }
    
    [Column("id_statusu")]
    public int StatusId { get; set; }
    
    public virtual Profil Profil { get; set; } = null!;
    
    [ForeignKey(nameof(StatusId))]
    public virtual Status Status { get; set; } = null!;
}