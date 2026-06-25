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
    public string? WartoscStatystyki { get; set; }
    public double? WartoscLiczbowaStatystyki { get; set; }
    public DateTime? OstatnieOtwarcieCzatu { get; set; }
    
    public virtual Druzyna Druzyna { get; set; } = null!;
    public virtual Uzytkownik? Uzytkownik { get; set; }
    public virtual Rola? Rola { get; set; }
    public virtual Statystyka? Statystyka { get; set; }
}