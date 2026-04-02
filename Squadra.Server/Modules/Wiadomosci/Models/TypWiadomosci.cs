namespace Squadra.Server.Modules.Wiadomosci.Models;

public class TypWiadomosci
{
    public int Id { get; set; }
    public string Nazwa { get; set; } = null!;

    public virtual ICollection<Wiadomosc> WiadomosciCollection { get; set; } = null!;
}