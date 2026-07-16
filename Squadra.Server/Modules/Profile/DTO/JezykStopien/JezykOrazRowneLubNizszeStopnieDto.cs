namespace Squadra.Server.Modules.Profile.DTO.JezykStopien;

public record JezykOrazRowneLubNizszeStopnieDto(
    JezykDto Jezyk,
    ICollection<StopienBieglosciJezykaDto> Stopnie
);