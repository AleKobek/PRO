using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Squadra.Server.Modules.Uzytkownicy.Models;

namespace Squadra.Server.Modules.Wiadomosci.Models;

public class Wiadomosc
{
    public int Id { get; set; }
    public int IdNadawcy { get; set; }
    public int IdOdbiorcy { get; set; }
    public DateTime DataWyslania { get; set; }
    public string Tresc { get; set; } = null!;
    public int IdTypuWiadomosci { get; set; }
    
    public virtual Uzytkownik Nadawca { get; set; } = null!;
    public virtual Uzytkownik Odbiorca { get; set; } = null!;
    public virtual TypWiadomosci TypWiadomosci { get; set; } = null!;

}