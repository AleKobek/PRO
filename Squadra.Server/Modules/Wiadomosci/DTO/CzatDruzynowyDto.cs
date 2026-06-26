using Squadra.Server.Modules.Profile.DTO.Profil;

namespace Squadra.Server.Modules.Wiadomosci.DTO;

public record CzatDruzynowyDto(
    ICollection<ProfilMinInfoDto> Uczestnicy,
    ICollection<WiadomoscDto> Wiadomosci
);