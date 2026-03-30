using Squadra.Server.Modules.Platformy.Models;

namespace Squadra.Server.Modules.BibliotekaGier.Models;

public class GraUzytkownikaNaPlatformie
{
    public int UzytkownikId { get; set; }
    public int GraId { get; set; }
    public int PlatformaId { get; set; }
    
    public virtual GraUzytkownika Gra { get; set; }
    public virtual Platforma Platforma { get; set; }
}