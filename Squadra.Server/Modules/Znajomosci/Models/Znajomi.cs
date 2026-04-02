using Squadra.Server.Modules.Uzytkownicy.Models;

namespace Squadra.Server.Modules.Znajomosci.Models;

public class Znajomi
{
    public int IdUzytkownika1 { get; set; }
    public int IdUzytkownika2 { get; set; }
    public DateOnly DataNawiazaniaZnajomosci { get; set; }
    public DateTime? OstatnieOtwarcieCzatuUzytkownika1 { get; set; }
    public DateTime? OstatnieOtwarcieCzatuUzytkownika2 { get; set; }
    
    
    public virtual Uzytkownik Uzytkownik1 { get; set; } = null!;
    public virtual Uzytkownik Uzytkownik2 { get; set; } = null!;
}