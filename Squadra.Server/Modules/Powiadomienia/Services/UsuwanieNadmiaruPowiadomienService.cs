using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Drużyny.Repositories;
using Squadra.Server.Modules.Powiadomienia.DTO;
using Squadra.Server.Modules.Powiadomienia.Enums;
using Squadra.Server.Modules.Powiadomienia.Repositories;
using Squadra.Server.Modules.Profile.Services;
using Squadra.Server.Modules.Statystyki.Services;

namespace Squadra.Server.Modules.Powiadomienia.Services;

public class UsuwanieNadmiaruPowiadomienService(IPowiadomienieRepository powiadomienieRepository,
    IProfilService profilService,
    IDruzynyRepository druzynyRepository, // importujemy repozytorium, aby nie powodować zapętlenia, a i tak jedyne co potrzebujemy to proste pobieranie z bazy
    IStatystykiService statystykiService
    ) : IUsuwanieNadmiaruPowiadomienService
{
    public async Task<bool> UsunNadmiarowePowiadomieniaUzytkownika(int idUzytkownika)
    {
        var powiadomieniaDoUsuniecia = await powiadomienieRepository.PodajPowiadomieniaUzytkownikaPrzekraczajaceLimit(idUzytkownika);
        if (powiadomieniaDoUsuniecia.Count == 0) return true;
        // w przypadku usuwania zaproszeń musimy wysłać powiadomienie zwrotne, że zaproszenie zostało odrzucone
        foreach (var powiadomienieDoUsuniecia in powiadomieniaDoUsuniecia)
        {
            if ((TypPowiadomieniaEnum)powiadomienieDoUsuniecia.TypPowiadomieniaId is TypPowiadomieniaEnum.ZaproszenieDoZnajomych)
            {
                var profilRes = await profilService.GetProfil(idUzytkownika);
                if (!profilRes.Succeeded) continue; // coś jest nie tak z powiadomieniem, więc nie wysyłamy powiadomienia zwrotnego, tylko usuwamy
                
                // wysyłamy powiadomienie do zapraszającego, że jego zaproszenie zostało odrzucone, bo nie ma miejsca w powiadomieniach
                var powiadomienieDoZapraszajacego = new PowiadomienieCreateDto(
                    (int)TypPowiadomieniaEnum.OdrzucenieZaproszeniaDoZnajomych,
                    powiadomienieDoUsuniecia.PowiazanyObiektId ?? 1, // nie będzie sytuacji tutaj, że to będzie null, ale aby się nie czepiał kompilator
                    idUzytkownika, // powiązany jest użytkownik, u którego jest to usuwane
                    profilRes.Value.Pseudonim,
                    null,
                    null,
                    null
                );
                await powiadomienieRepository.CreatePowiadomienie(powiadomienieDoZapraszajacego); // nie uruchamiamy jeszcze raz w serwisie, bo to by mogło spowodować pętlę, więc wywołujemy repozytorium bezpośrednio
            }

            if ((TypPowiadomieniaEnum)powiadomienieDoUsuniecia.TypPowiadomieniaId is TypPowiadomieniaEnum.ZaproszenieDoDruzyny)
            {
                try
                {
                    var druzyna = await druzynyRepository.GetDruzyna(powiadomienieDoUsuniecia.PowiazanyObiektId ?? 1); // nie będzie sytuacji tutaj, że to będzie null, ale aby się nie czepiał kompilator

                    // pobieramy miejsce w drużynie
                    var miejsce = await druzynyRepository.GetMiejsceWDruzynie(powiadomienieDoUsuniecia.DrugiPowiazanyObiektId ?? 1);
                    
                    string? nazwaRoli = null;
                    if (miejsce.RolaId != null)
                    {
                        var rolaRes = await statystykiService.GetRola(miejsce.RolaId ?? 1);
                        if (rolaRes.Succeeded && rolaRes.Value != null) nazwaRoli = rolaRes.Value.Nazwa;
                    }

                    var profilRes = await profilService.GetProfil(idUzytkownika);
                    if (!profilRes.Succeeded)
                        continue;

                    var powiadomienieDoKapitana = new PowiadomienieCreateDto(
                        (int)TypPowiadomieniaEnum.OdrzucenieZaproszeniaDoDruzyny,
                        druzyna.KapitanId,
                        idUzytkownika, // powiązany jest użytkownik, u którego jest to usuwane
                        profilRes.Value.Pseudonim,
                        druzyna.Id,
                        druzyna.Nazwa,
                        nazwaRoli
                    );
                    
                    // nie uruchamiamy jeszcze raz w serwisie, bo to by mogło spowodować pętlę, więc wywołujemy repozytorium bezpośrednio}
                    await powiadomienieRepository.CreatePowiadomienie(powiadomienieDoKapitana);
                }
                catch (NieZnalezionoWBazieException e)
                {
                    continue; // coś jest nie tak z powiadomieniem, więc nie wysyłamy powiadomienia zwrotnego, tylko usuwamy
                }
            }
        }
        await powiadomienieRepository.UsunPowiadomienia(powiadomieniaDoUsuniecia);
        return true;
    }
}