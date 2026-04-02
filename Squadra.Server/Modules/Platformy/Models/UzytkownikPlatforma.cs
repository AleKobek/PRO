using Squadra.Server.Modules.Uzytkownicy.Models;

namespace Squadra.Server.Modules.Platformy.Models;

public class UzytkownikPlatforma
{
    public int UzytkownikId { get; set; }
    public int PlatformaId { get; set; }
    public string PseudonimNaPlatformie { get; set; } = null!;
    
    public virtual Platforma Platforma { get; set; }
    public virtual Uzytkownik Uzytkownik { get; set; }
}