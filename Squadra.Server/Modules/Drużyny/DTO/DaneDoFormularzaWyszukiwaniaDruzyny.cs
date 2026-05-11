using Squadra.Server.Modules.Platformy.DTO;
using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.WspieraneGry.DTO;

namespace Squadra.Server.Modules.Drużyny.DTO;

public record DaneDoFormularzaWyszukiwaniaDruzyny(
    ICollection<MinInfoWspieranaGraDTO> WszystkieGry,
    ICollection<MinInfoWspieranaGraDTO> GryUzytkownika,
    ICollection<NastrojRozgrywkiDto> NastrojeRozgrywki,
    ICollection<GraZPlatformaDTO> GryZPlatformami,
    ICollection<JezykOrazRowneLubNizszeStopnieDto> JezykiOrazStopnie,
    ICollection<RolaDto> Role
);