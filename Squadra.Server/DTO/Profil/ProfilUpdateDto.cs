using Squadra.Server.DTO.JezykStopien;

namespace Squadra.Server.DTO.Profil;

public record ProfilUpdateDto(
    int? RegionId,
    string? Zaimki,
    string? Opis,
    ICollection<JezykProfiluCreateDto> Jezyki,
    string Pseudonim
);