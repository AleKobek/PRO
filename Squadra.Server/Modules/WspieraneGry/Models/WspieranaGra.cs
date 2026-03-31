using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Squadra.Server.Modules.BibliotekaGier.Models;
using Squadra.Server.Modules.Statystyki.Models;

namespace Squadra.Server.Modules.WspieraneGry.Models;

[Table(nameof(WspieranaGra))]
public class WspieranaGra
{
    [Key]
    public int Id { get; set; }
    
    [Required][MaxLength(60)]
    public string Tytul { get; set; } = null!;
    
    [Required][MaxLength(30)]
    public string Wydawca { get; set; } = null!;
    
    [Required][MaxLength(30)]
    public string Gatunek { get; set; } = null!;
    
    public virtual ICollection<GraNaPlatformie> GraNaPlatformieCollection { get; set; } = null!;
    public virtual ICollection<GraUzytkownika> GraUzytkownikaCollection { get; set; } = null!;
    public virtual ICollection<Kategoria> KategoriaCollection { get; set; } = null!;
    public virtual ICollection<Rola> RolaCollection { get; set; } = null!;
}