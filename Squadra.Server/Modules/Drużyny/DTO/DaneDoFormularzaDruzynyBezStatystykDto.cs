using Squadra.Server.Modules.Drużyny.Models;
using Squadra.Server.Modules.Platformy.Models;
using Squadra.Server.Modules.Profile.Models;
using Squadra.Server.Modules.Statystyki.Models;

namespace Squadra.Server.Modules.Drużyny.DTO;

public record DaneDoFormularzaDruzynyBezStatystykDto(
    ICollection<NastrojRozgrywki> NastrojeRozgrywki,
    ICollection<Platforma> Platformy,
    ICollection<Jezyk> Jezyki,
    ICollection<StopienBieglosciJezyka> StopnieBieglosciJezyka,
    ICollection<Rola> Role
);