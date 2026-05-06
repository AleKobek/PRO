using Squadra.Server.Modules.Profile.Models;

namespace Squadra.Server.Modules.Profile.DTO.JezykStopien;

public record JezykOrazRowneLubNizszeStopnieDto(
    Jezyk Jezyk,
    ICollection<StopienBieglosciJezyka> Stopnie
);