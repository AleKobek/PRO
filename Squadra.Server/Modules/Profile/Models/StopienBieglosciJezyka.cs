namespace Squadra.Server.Modules.Profile.Models;

public class StopienBieglosciJezyka
{
    public int Id { get; set; }
    public string Nazwa { get; set; } = null!;
    public int Wartosc { get; set; }
    
    public virtual ICollection<JezykProfilu> JezykProfiluCollection { get; set; } = null!;
 
}