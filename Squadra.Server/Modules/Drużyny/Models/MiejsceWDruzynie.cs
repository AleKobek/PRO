using Squadra.Server.Modules.Statystyki.Models;
using Squadra.Server.Modules.Uzytkownicy.Models;

namespace Squadra.Server.Modules.Drużyny.Models;

public class MiejsceWDruzynie
{
    public int Id { get; set; }
    public int DruzynaId { get; set; }
    public int? UzytkownikId { get; set; }
    public int? RolaId { get; set; }
    public int? StatystykaId { get; set; }
    public string? WartoscStatystyki { get; set; } // do wyświetlania
    public double? WartoscLiczbowaStatystyki { get; set; } // do porównywania
    public DateTime? OstatnieOtwarcieCzatu { get; set; } // data ostatniego otwarcia czatu przez użytkownika na tym miejscu
    
    public virtual Druzyna Druzyna { get; set; } = null!;
    public virtual Uzytkownik? Uzytkownik { get; set; }
    public virtual Rola? Rola { get; set; }
    public virtual Statystyka? Statystyka { get; set; }
}