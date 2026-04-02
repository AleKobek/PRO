namespace Squadra.Server.Modules.Profile.Models;
public class Region
{
    public int Id { get; set; }
    public string Nazwa { get; set; } = null!;
    // region musi mieć kraj! nie będzie regionu "brak"
    public int KrajId { get; set; }
    
    public virtual Kraj Kraj { get; set; } = null!;
    public virtual ICollection<Profil> ProfilCollection { get; set; } = null!;
}