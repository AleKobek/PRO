using Squadra.Server.Modules.Drużyny.Services;
using Squadra.Server.Modules.Powiadomienia.Enums;
using Squadra.Server.Modules.Powiadomienia.Repositories;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Powiadomienia.Services;

public class UsunPowiadomieniaUzytkownikaService(IPowiadomieniaRepository powiadomieniaRepository, IDruzynyService druzynyService) : IUsunPowiadomieniaUzytkownikaService
{
    // uruchamiamy to przy usuwaniu konta
    public async Task<ServiceResult<bool>> DeletePowiadomieniaZwiazaneZUzytkownikiem(int idUzytkownika)
    {
        if(idUzytkownika < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id użytkownika: " + idUzytkownika));
        
        // wysyłamy kapitanom druzyny, od których ma zaproszenia, że ich zaproszenie zostanie usunięte.
        var zaproszeniaDoDruzyny = await powiadomieniaRepository.GetZaproszeniaDoDruzynyUzytkownika(idUzytkownika);
        foreach (var zaproszenie in zaproszeniaDoDruzyny)
        {
            // w zaproszeniu pierwszym powiązanym obiektem jest drużyna, drugim jest miejsce (bez nazwy). W treści jest rola miejsca
            // musimy wysłać do kapitana drużyny powiadomienie, że jego zaproszenie na dane miejsce zostało usunięte.
            // potrzebujemy drużyny (aby mieć kapitana i nazwę) oraz miejsce (aby mieć numer miejsca)
            var druzynaRes = zaproszenie.PowiazanyObiektId != null ? await druzynyService.GetDruzyna(zaproszenie.PowiazanyObiektId.Value) : null;
            if(druzynaRes == null || !druzynaRes.Succeeded) continue; // drużyna już nie istnieje lub coś jest nie tak, więc nie ma sensu wysyłać powiadomienia
            var miejsceRes = zaproszenie.DrugiPowiazanyObiektId != null ? await druzynyService.GetNumerMiejsca(zaproszenie.DrugiPowiazanyObiektId.Value) : null;
            if(miejsceRes == null || !miejsceRes.Succeeded) continue; // miejsce nie istnieje lub coś jest nie tak, więc nie ma sensu wysyłać powiadomienia
            
            await powiadomieniaRepository.CreatePowiadomienie(new DTO.PowiadomienieCreateDto(
                (int)TypPowiadomieniaEnum.ZaproszenieOdrzuconePrzezUsuniecieKonta,
                druzynaRes.Value.KapitanId,
                druzynaRes.Value.Id,
                druzynaRes.Value.Nazwa,
                zaproszenie.DrugiPowiazanyObiektId,
                miejsceRes.Value.ToString(),
                zaproszenie.Tresc
            ));
        }
        
        // jak tu dochodzimy, wszystko jest w porządku
        return ServiceResult<bool>.NoContent(await powiadomieniaRepository.DeletePowiadomieniaZwiazaneZUzytkownikiem(idUzytkownika));
    }
}