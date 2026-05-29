using Squadra.Server.Modules.Platformy.Models;
using Squadra.Server.Modules.Profile.Models;
using Squadra.Server.Modules.Statystyki.Models;
using Squadra.Server.Modules.Uzytkownicy.Models;
using Squadra.Server.Modules.WspieraneGry.Models;

namespace Squadra.Server.Modules.Drużyny.Models;

public class Druzyna
{
    public int Id { get; set; }
    public string Nazwa { get; set; } = null!;
    public int GraId { get; set; }
    public int KapitanId { get; set; }
    public bool CzyPubliczna { get; set; }
    public string? Opis { get; set; }
    public int NastrojRozgrywkiId { get; set; }
    public int? WymaganyJezykId { get; set; }
    public int? WymaganyStopienBieglosciJezykaId { get; set; }
    public int? PlatformaId { get; set; }
    public bool CzyMaWymagania { get; set; }
    
    public virtual WspieranaGra Gra { get; set; } = null!;
    public virtual Uzytkownik Kapitan { get; set; } = null!;
    public virtual NastrojRozgrywki NastrojRozgrywki { get; set; } = null!;
    public virtual Jezyk WymaganyJezyk { get; set; } = null!;
    public virtual StopienBieglosciJezyka WymaganyStopienBieglosciJezyka { get; set; } = null!;
    public virtual Platforma Platforma { get; set; } = null!;
    public virtual ICollection<MiejsceWDruzynie> MiejsceWDruzynieCollection { get; set; } = new List<MiejsceWDruzynie>();
    public virtual ICollection<WymaganaStatystykaDruzyny> WymaganaStatystykaDruzynyCollection { get; set; } = new List<WymaganaStatystykaDruzyny>();
}