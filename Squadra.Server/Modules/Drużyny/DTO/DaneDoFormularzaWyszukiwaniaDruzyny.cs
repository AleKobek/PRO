using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Statystyki.DTO;
using Squadra.Server.Modules.WspieraneGry.DTO;

namespace Squadra.Server.Modules.Drużyny.DTO;

public record DaneDoFormularzaWyszukiwaniaDruzyny(
    ICollection<GraZPlatformaDoSelectDto> WszystkieGryzPlatformami,
    ICollection<GraZPlatformaDoSelectDto> GryUzytkownikaZPlatformami,
    ICollection<NastrojRozgrywkiDto> NastrojeRozgrywki,
    ICollection<JezykOrazRowneLubNizszeStopnieDto> JezykiOrazStopnie,
    ICollection<RolaDto> Role
);