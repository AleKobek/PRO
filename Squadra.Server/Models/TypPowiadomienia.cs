using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Squadra.Server.Models;

[Table(nameof(TypPowiadomienia))]
public class TypPowiadomienia
{
    public int Id { get; set; }
    
    [Required] [StringLength(100)]
    public string Nazwa { get; set; } = null!;
    
    public virtual ICollection<Powiadomienie> PowiadomienieCollection { get; set; } = null!;

}

/*
 
 Typy jak na razie:
    -> systemowe - nie mają id powiązanego obiektu, bo są od systemu do użytkownika. po prostu przekazywana jest treść
        -> np powiadomienia odnośnie odrzucenia zaproszenia do znajomych, też chyba nie trzeba robić powiązania
        -> interakcja to usuń
    -> zaproszenia do znajomych - powiązane z obiektem innego użytkownika, 
        -> interakcja to odpowiedź tak lub nie
 */