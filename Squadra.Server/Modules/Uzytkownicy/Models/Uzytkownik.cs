using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Squadra.Server.Modules.BibliotekaGier.Models;
using Squadra.Server.Modules.Platformy.Models;
using Squadra.Server.Modules.Powiadomienia.Models;
using Squadra.Server.Modules.Profile.Models;
using Squadra.Server.Modules.Wiadomosci.Models;
using Squadra.Server.Modules.Znajomosci.Models;

namespace Squadra.Server.Modules.Uzytkownicy.Models;

public class Uzytkownik : IdentityUser<int>
{
    public DateOnly? DataUrodzenia { get; set; }
    
    public DateTime? OstatniaAktywnosc { get; set; }
    
    [MaxLength(40)]
    public string? LoginNaZewnetrznymSerwisie { get; set; }
    
    public int? IdNaZewnetrznymSerwisie { get; set; }
    
    // jest nullable, bo profil może być tworzony tuż po użytkowniku
    public virtual Profil? Profil { get; set; } = null!;
    
    public virtual ICollection<Powiadomienie> PowiadomienieCollection { get; set; } = null!;
    
    public virtual ICollection<Znajomi> ZnajomiJakoPierwszyCollection { get; set; } = null!;
    public virtual ICollection<Znajomi> ZnajomiJakoDrugiCollection { get; set; } = null!;
    public virtual ICollection<Wiadomosc> WiadomosciOdebraneCollection { get; set; } = null!;
    public virtual ICollection<Wiadomosc> WiadomosciNadaneCollection { get; set; } = null!;
    public virtual ICollection<UzytkownikPlatforma> UzytkownikPlatformaCollection { get; set; } = null!;
    public virtual ICollection<GraUzytkownika> GraUzytkownikaCollection { get; set; } = null!;
}