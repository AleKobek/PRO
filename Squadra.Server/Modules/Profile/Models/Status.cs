namespace Squadra.Server.Modules.Profile.Models;

public class Status
{
    public int Id { get; set; }
    public string Nazwa { get; set; } = null!;
    
    public virtual ICollection<Profil> ProfilCollection { get; set; } = null!;
}