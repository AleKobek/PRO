using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Squadra.Server.Modules.Uzytkownicy.Models;
using Squadra.Server.Modules.WspieraneGry.Models;

namespace Squadra.Server.Modules.BibliotekaGier.Models;

[Table(nameof(GraUzytkownika))]
[PrimaryKey(nameof(UzytkownikId), nameof(GraId))]
public class GraUzytkownika
{
    public int UzytkownikId { get; set; }
    public int GraId { get; set; }
    
    [ForeignKey(nameof(GraId))]
    public virtual WspieranaGra Gra { get; set; }
    
    [ForeignKey(nameof(UzytkownikId))]
    public virtual Uzytkownik Uzytkownik { get; set; }
}