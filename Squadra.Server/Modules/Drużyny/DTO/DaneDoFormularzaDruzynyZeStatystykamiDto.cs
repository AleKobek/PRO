using Squadra.Server.Modules.Drużyny.Models;
using Squadra.Server.Modules.Platformy.Models;
using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Statystyki.DTO;
using Squadra.Server.Modules.Statystyki.Models;

namespace Squadra.Server.Modules.Drużyny.DTO;

public record DaneDoFormularzaDruzynyZeStatystykamiDto(
    ICollection<NastrojRozgrywki> NastrojeRozgrywki,
    ICollection<Platforma> Platformy,
    ICollection<JezykOrazRowneLubNizszeStopnieDto> JezykiOrazStopnie,
    ICollection<Rola> Role,
    StatystykiDoFormularzaDto Statystyki
);