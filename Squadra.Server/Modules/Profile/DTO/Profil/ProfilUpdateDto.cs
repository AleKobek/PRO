using Squadra.Server.Modules.Profile.DTO.JezykStopien;

namespace Squadra.Server.Modules.Profile.DTO.Profil;

public record ProfilUpdateDto(
    int? RegionId,
    string? Zaimki,
    string? Opis,
    ICollection<JezykProfiluCreateDto> Jezyki,
    string Pseudonim
);