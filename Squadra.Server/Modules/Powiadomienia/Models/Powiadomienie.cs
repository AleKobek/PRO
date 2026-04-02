using Squadra.Server.Modules.Uzytkownicy.Models;

namespace Squadra.Server.Modules.Powiadomienia.Models;

public class Powiadomienie
{
    public int Id { get; set; }
    public int TypPowiadomieniaId { get; set; }
    public int UzytkownikId { get; set; }
    public int? PowiazanyObiektId { get; set; }
    public string? PowiazanyObiektNazwa { get; set; }
    // treść jest tylko dla systemowych, reszta jest tworzona na miejscu
    public string? Tresc { get; set; }
    public DateTime DataWyslania { get; set; }
    public virtual TypPowiadomienia TypPowiadomienia { get; set; } = null!;
    public virtual Uzytkownik Uzytkownik { get; set; } = null!;
}