using Squadra.Server.Modules.Uzytkownicy.Models;

namespace Squadra.Server.Modules.Statystyki.Models;

public class StatystykaUzytkownika
{
    public int UzytkownikId { get; set; }
    public int StatystykaId { get; set; }
    public int Wartosc { get; set; }
    
    public virtual Statystyka Statystyka { get; set; }
    public virtual Uzytkownik Uzytkownik { get; set; }
}