using Squadra.Server.DTO.JezykStopien;
using Squadra.Server.DTO.KrajRegion;
using Squadra.Server.DTO.Status;

namespace Squadra.Server.DTO.Profil;

public record ProfilGetResDto
(
    string Pseudonim,
    RegionKrajDto? RegionIKraj,
    string? Zaimki,
    string? Opis,
    ICollection<JezykOrazStopienDto> Jezyki,
    byte[]? Awatar
);