using Squadra.Server.Modules.Drużyny.Models;

namespace Squadra.Server.Modules.Statystyki.Models;

public class Statystyka
{
    public int Id { get; set; }
    public string Nazwa { get; set; }
    public int KategoriaId { get; set; }
    public int? RolaId { get; set; }
    public bool CzyToCzasRozgrywki { get; set; }
    
    public virtual Kategoria Kategoria { get; set; }
    public virtual Rola? Rola { get; set; }
    public virtual ICollection<StatystykaUzytkownika> StatystykaUzytkownikaCollection { get; set; } = new List<StatystykaUzytkownika>();
    public virtual ICollection<MiejsceWDruzynie> MiejsceWDruzynieCollection { get; set; } = new List<MiejsceWDruzynie>();
    public virtual ICollection<WymaganaStatystykaDruzyny> WymaganaStatystykaDruzynyCollection { get; set; } = new List<WymaganaStatystykaDruzyny>();
    public virtual ICollection<Ranga> RangaCollection { get; set; } = new List<Ranga>();
}