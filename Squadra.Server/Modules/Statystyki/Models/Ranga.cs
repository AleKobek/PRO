namespace Squadra.Server.Modules.Statystyki.Models;

// pomocnicza klasa do uzupełniania wartości statystyki, to samo co w zewnętrznych
public class Ranga
{
    public int StatystykaId { get; set; }
    public int WartoscLiczbowa { get; set; }
    public string Nazwa { get; set; }
    
    public virtual Statystyka Statystyka { get; set; }
}