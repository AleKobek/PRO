using Squadra.Server.Modules.Uzytkownicy.Models;

namespace Squadra.Server.Modules.Profile.Models;

public class Profil
{
    public int IdUzytkownika { get; set; }
    public string? Zaimki { get; set; }
    public string Pseudonim { get; set; } = null!;
    public string? Opis { get; set; }
    public int? RegionId { get; set; } 
    public byte[]? Awatar { get; set; }
    public int StatusId { get; set; }
    
    public virtual Uzytkownik Uzytkownik { get; set; } = null!;
    public virtual Region? Region { get; set; } = null!;
    public virtual ICollection<JezykProfilu> JezykProfiluCollection { get; set; } = null!;
    public virtual Status Status { get; set; } = null!;
}