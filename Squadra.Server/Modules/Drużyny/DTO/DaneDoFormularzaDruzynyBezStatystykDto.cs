using Squadra.Server.Modules.Platformy.DTO;
using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Statystyki.DTO;

namespace Squadra.Server.Modules.Drużyny.DTO;

public record DaneDoFormularzaDruzynyBezStatystykDto(
    ICollection<NastrojRozgrywkiDto> NastrojeRozgrywki,
    ICollection<PlatformaDto> Platformy,
    ICollection<JezykOrazRowneLubNizszeStopnieDto> JezykiOrazStopnie,
    ICollection<RolaDto> Role
);