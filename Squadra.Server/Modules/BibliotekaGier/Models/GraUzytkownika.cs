using Squadra.Server.Modules.Uzytkownicy.Models;
using Squadra.Server.Modules.WspieraneGry.Models;

namespace Squadra.Server.Modules.BibliotekaGier.Models;

public class GraUzytkownika
{
    public int UzytkownikId { get; set; }
    public int GraId { get; set; }
    public virtual WspieranaGra Gra { get; set; }
    
    public virtual Uzytkownik Uzytkownik { get; set; }
    public virtual ICollection<GraUzytkownikaNaPlatformie> GraUzytkownikaNaPlatformieCollection { get; set; }
}