using Microsoft.AspNetCore.Identity;

namespace Squadra.Server.Models;

public class Uzytkownik : IdentityUser<int>
{
    public DateOnly? DataUrodzenia { get; set; }
    
    // jest nullable, bo profil może być tworzony tuż po użytkowniku
    public virtual Profil? Profil { get; set; } = null!;
}