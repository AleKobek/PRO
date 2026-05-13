using Squadra.Server.Modules.Drużyny.Models;

namespace Squadra.Server.Modules.Statystyki.Models;

public class WymaganaStatystykaDruzyny
{
    public int DruzynaId { get; set; }
    public int StatystykaId { get; set; }
    public string? Wartosc { get; set; } = null!;
    public double? PorownywalnaWartoscLiczbowa { get; set; }
    
    public virtual Druzyna Druzyna { get; set; } = null!;
    public virtual Statystyka Statystyka { get; set; } = null!;
}