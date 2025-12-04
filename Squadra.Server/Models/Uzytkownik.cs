using Microsoft.AspNetCore.Identity;

namespace Squadra.Server.Models;

public class Uzytkownik : IdentityUser<int>
{
    public DateOnly? DataUrodzenia { get; set; }
    
    public DateTime? OstatniaAktywnosc { get; set; }
    
    // jest nullable, bo profil może być tworzony tuż po użytkowniku
    public virtual Profil? Profil { get; set; } = null!;
    
    public virtual ICollection<Powiadomienie> PowiadomienieCollection { get; set; } = null!;
    
    public virtual ICollection<Znajomi> ZnajomiJakoPierwszyCollection { get; set; } = null!;
    public virtual ICollection<Znajomi> ZnajomiJakoDrugiCollection { get; set; } = null!;
}