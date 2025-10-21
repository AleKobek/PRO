using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squadra.Server.Models;

[Table(nameof(Uzytkownik))]
public class Uzytkownik
{
    [Key]
    public int Id { get; set; }

    [MaxLength(20)] [Required] 
    public string Login { get; set; } = null!;
    
   
    // znormalizowany 
    [MaxLength(64)] [Required] [EmailAddress]
    public string Email { get; set; } = null!;
    
    [MaxLength(256)] [Required]
    public string Haslo { get; set; } = null!;
    
    [MaxLength(15)]
    public string? NumerTelefonu { get; set; }
    
    public DateOnly? DataUrodzenia { get; set; }
    
    public virtual Profil Profil { get; set; } = null!;
    
}