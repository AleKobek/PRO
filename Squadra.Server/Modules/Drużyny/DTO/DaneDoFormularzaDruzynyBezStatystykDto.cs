using Squadra.Server.Modules.Platformy.Models;
using Squadra.Server.Modules.Profile.DTO.JezykStopien;

namespace Squadra.Server.Modules.Drużyny.DTO;

public record DaneDoFormularzaDruzynyBezStatystykDto(
    ICollection<NastrojRozgrywkiDto> NastrojeRozgrywki,
    ICollection<Platforma> Platformy,
    ICollection<JezykDto> Jezyki,
    ICollection<StopienBieglosciJezykaDto> StopnieBieglosciJezyka,
    ICollection<RolaDto> Role
);