namespace Squadra.Server.Modules.Statystyki.Models;

public class Statystyka
{
    public int Id { get; set; }
    public string Nazwa { get; set; }
    public int KategoriaId { get; set; }
    public int RolaId { get; set; }
    
    public virtual Kategoria Kategoria { get; set; }
    public virtual Rola Rola { get; set; }
    public virtual ICollection<StatystykaUzytkownika> StatystykaUzytkownikaCollection { get; set; }
}