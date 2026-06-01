using Squadra.Server.Modules.Platformy.DTO;
using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.WspieraneGry.DTO;

namespace Squadra.Server.Modules.Drużyny.DTO;

public record DaneDoFormularzaWyszukiwaniaDruzyny(
    ICollection<GraZPlatformaDoSelectDto> WszystkieGryzPlatformami,
    ICollection<GraZPlatformaDoSelectDto> GryUzytkownikaZPlatformami,
    ICollection<NastrojRozgrywkiDto> NastrojeRozgrywki,
    ICollection<JezykOrazRowneLubNizszeStopnieDto> JezykiOrazStopnie,
    ICollection<RolaDto> Role
);