namespace Squadra.Server.Modules.Profile.Models;

public class JezykProfilu
{
 
    public int UzytkownikId { get; set; }
    public int JezykId { get; set; }
    public int StopienBieglosciId { get; set; }
    
    public virtual Profil Profil { get; set; } = null!;
    public virtual Jezyk Jezyk { get; set; } = null!;
    public virtual StopienBieglosciJezyka StopienBieglosciJezyka { get; set; } = null!;
}