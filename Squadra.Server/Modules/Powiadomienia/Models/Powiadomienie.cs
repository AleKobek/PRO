using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squadra.Server.Models;

[Table(nameof(Powiadomienie))]
public class Powiadomienie
{
    [Key]
    public int Id { get; set; }
    public int TypPowiadomieniaId { get; set; }
    public int UzytkownikId { get; set; }
    public int? PowiazanyObiektId { get; set; }
    
    [MaxLength(30)]
    public string? PowiazanyObiektNazwa { get; set; }
    
    [MaxLength(200)]
    // treść jest tylko dla systemowych, reszta jest tworzona na miejscu
    public string? Tresc { get; set; }
    public DateTime DataWyslania { get; set; }
    public virtual TypPowiadomienia TypPowiadomienia { get; set; } = null!;
    public virtual Uzytkownik Uzytkownik { get; set; } = null!;
}