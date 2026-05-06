using Squadra.Server.Modules.Drużyny.Models;
using Squadra.Server.Modules.Platformy.Models;
using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Statystyki.DTO;

namespace Squadra.Server.Modules.Drużyny.DTO;

public record DaneDoFormularzaDruzynyZeStatystykamiDto(
    ICollection<NastrojRozgrywkiDto> NastrojeRozgrywki,
    ICollection<Platforma> Platformy,
    ICollection<JezykOrazRowneLubNizszeStopnieDto> JezykiOrazStopnie,
    ICollection<RolaDto> Role,
    StatystykiDoFormularzaDto Statystyki
);