using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Squadra.Server.Modules.Platforma.Models;

namespace Squadra.Server.Models;

public class Uzytkownik : IdentityUser<int>
{
    public DateOnly? DataUrodzenia { get; set; }
    
    public DateTime? OstatniaAktywnosc { get; set; }
    
    [MaxLength(40)]
    // tylko na potrzeby symulacji tamtego serwisu
    public string? LoginNaZewnetrznymSerwisie { get; set; }
    
    [MaxLength(128)]
    // tylko na potrzeby symulacji tamtego serwisu
    public string? HasloNaZewnetrznymSerwisieHash { get; set; }
    
    public int? IdNaZewnetrznymSerwisie { get; set; }
    
    // jest nullable, bo profil może być tworzony tuż po użytkowniku
    public virtual Profil? Profil { get; set; } = null!;
    
    public virtual ICollection<Powiadomienie> PowiadomienieCollection { get; set; } = null!;
    
    public virtual ICollection<Znajomi> ZnajomiJakoPierwszyCollection { get; set; } = null!;
    public virtual ICollection<Znajomi> ZnajomiJakoDrugiCollection { get; set; } = null!;
    public virtual ICollection<Wiadomosc> WiadomosciOdebraneCollection { get; set; } = null!;
    public virtual ICollection<Wiadomosc> WiadomosciNadaneCollection { get; set; } = null!;
    public virtual ICollection<UzytkownikPlatforma> UzytkownikPlatformaCollection { get; set; } = null!;
}