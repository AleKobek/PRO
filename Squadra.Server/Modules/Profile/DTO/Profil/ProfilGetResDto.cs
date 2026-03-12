using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Profile.DTO.KrajRegion;

namespace Squadra.Server.Modules.Profile.DTO.Profil;

public record ProfilGetResDto
(
    string Pseudonim,
    RegionKrajDto? RegionIKraj,
    string? Zaimki,
    string? Opis,
    ICollection<JezykOrazStopienDto> Jezyki,
    byte[]? Awatar,
    string NazwaStatusu
);