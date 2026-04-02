using Squadra.Server.Modules.BibliotekaGier.Models;
using Squadra.Server.Modules.WspieraneGry.Models;

namespace Squadra.Server.Modules.Platformy.Models;

public class Platforma
{
    public int Id { get; set; }
    public string Nazwa { get; set; } = null!;
    public byte[] Logo { get; set; } = null!;
    
    public virtual ICollection<UzytkownikPlatforma> UzytkownikPlatformaCollection { get; set; } = null!;
    public virtual ICollection<GraNaPlatformie> GraNaPlatformieCollection { get; set; } = null!;
    public virtual ICollection<GraUzytkownikaNaPlatformie> GraUzytkownikaNaPlatformieCollection { get; set; } = null!;
}