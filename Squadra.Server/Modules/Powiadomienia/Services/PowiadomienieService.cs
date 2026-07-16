using Microsoft.IdentityModel.Tokens;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Drużyny.Repositories;
using Squadra.Server.Modules.Powiadomienia.DTO;
using Squadra.Server.Modules.Powiadomienia.Enums;
using Squadra.Server.Modules.Powiadomienia.Repositories;
using Squadra.Server.Modules.Profile.Services;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Statystyki.Services;
using Squadra.Server.Modules.Uzytkownicy.Services;
using Squadra.Server.Modules.Znajomosci.Repositories;
using Squadra.Server.Modules.Znajomosci.Services;

namespace Squadra.Server.Modules.Powiadomienia.Services;

public class PowiadomienieService(IPowiadomienieRepository powiadomienieRepository,
    IUzytkownikService uzytkownikService,
    IZnajomiService znajomiService,
    IZnajomiRepository znajomiRepository,
    IProfilService profilService,
    IStatystykiService statystykiService,
    IDruzynyRepository druzynyRepository // importujemy repozytorium, aby nie powodować zapętlenia, a i tak jedyne co potrzebujemy to proste pobieranie z bazy
    ) : IPowiadomienieService
{
    public async Task<ServiceResult<PowiadomienieDto>> GetPowiadomienie(int id) {
        if(id < 1) return ServiceResult<PowiadomienieDto>.BadRequest(new ErrorItem("Nieprawidłowe id powiadomienia: " + id));
        var powiadomienie = await powiadomienieRepository.GetPowiadomienie(id);
        
        return ServiceResult<PowiadomienieDto>.Ok(new PowiadomienieDto(
            powiadomienie.Id,
            powiadomienie.TypPowiadomieniaId,
            powiadomienie.UzytkownikId,
            powiadomienie.PowiazanyObiektId,
            powiadomienie.PowiazanyObiektNazwa,
            powiadomienie.DrugiPowiazanyObiektId,
            powiadomienie.DrugiPowiazanyObiektNazwa,
            powiadomienie.Tresc,
            powiadomienie.DataWyslania.ToString("dd.MM.yyyy HH:mm")
        ));
    }

    public async Task<ServiceResult<ICollection<PowiadomienieDto>>> GetPowiadomieniaUzytkownika(int idUzytkownika)
    {
        // czy to dobry użytkownik sprawdzamy już w controllerze, bo mamy od razu id
        var powiadomienia = await powiadomienieRepository.GetPowiadomieniaUzytkownika(idUzytkownika);
        return ServiceResult<ICollection<PowiadomienieDto>>.Ok(powiadomienia.Select(x => new PowiadomienieDto(
            x.Id,
            x.TypPowiadomieniaId,
            x.UzytkownikId,
            x.PowiazanyObiektId,
            x.PowiazanyObiektNazwa,
            x.DrugiPowiazanyObiektId,
            x.DrugiPowiazanyObiektNazwa,
            x.Tresc,
            x.DataWyslania.ToString("dd.MM.yyyy HH:mm")
        )).ToList());
    }

    public async Task<ServiceResult<bool>> CzyUzytkownikMaPowiadomienieDanegoTypuPowiazaneZObiektami(int idUzytkownika, int idTypu, int idPowiazanegoObiektu, int? idDrugiegoPowiazanegoObiektu)
    {
        if(idUzytkownika < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id użytkownika: " + idUzytkownika));
        if(idPowiazanegoObiektu < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id powiązanego obiektu: " + idPowiazanegoObiektu));
        if(idDrugiegoPowiazanegoObiektu < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id powiązanego obiektu: " + idPowiazanegoObiektu));
        if (!Enum.IsDefined(typeof(TypPowiadomieniaEnum), idTypu)) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowy typ powiadomienia: " + idTypu));
        try
        {
            return ServiceResult<bool>.Ok(
                await powiadomienieRepository.CzyUzytkownikMaPowiadomienieDanegoTypuPowiazaneZObiektami(idUzytkownika, (int)TypPowiadomieniaEnum.ZaproszenieDoDruzyny ,idPowiazanegoObiektu, idDrugiegoPowiazanegoObiektu));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<bool>> CreatePowiadomienie(PowiadomienieCreateDto powiadomienie)
    {
        // okolicznościami tworzenia powiadomienia zajmują się inne klasy, tutaj tylko tworzymy
        if (powiadomienie.IdTypuPowiadomienia < 1)
            return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowy typ powiadomienia: " + powiadomienie.IdTypuPowiadomienia));
        if (powiadomienie.IdPowiazanegoObiektu < 1)
            return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id powiązanego obiektu: " + powiadomienie.IdPowiazanegoObiektu));

        // sprawdzamy, czy odnosi się do istniejącego obiektu

        // odnośnie znajomych (nowe zaproszenie, zaakceptowano zaproszenie, odrzucono zaproszenie, usunięto ze znajomych)
        if ((TypPowiadomieniaEnum)powiadomienie.IdTypuPowiadomienia is 
            TypPowiadomieniaEnum.ZaproszenieDoZnajomych 
            or TypPowiadomieniaEnum.PrzyjecieZaproszeniaDoZnajomych 
            or TypPowiadomieniaEnum.OdrzucenieZaproszeniaDoZnajomych 
            or TypPowiadomieniaEnum.UsuniecieZnajomosci
        ) {
            // nie będzie sytuacji tutaj, że to będzie null, ale aby się nie czepiał kompilator
            var idUzytkownika = powiadomienie.IdPowiazanegoObiektu ?? 1;
            // sprawdzamy, czy taki użytkownik istnieje
            var wynikZnalezieniaUzytkownika = await uzytkownikService.GetUzytkownik(idUzytkownika);
            if (wynikZnalezieniaUzytkownika.StatusCode != 200)
                return ServiceResult<bool>.NotFound(wynikZnalezieniaUzytkownika.Errors[0]);
        }
        
        // pierwszym powiązanym obiektem jest użytkownik, drugim powiązanym obiektem jest drużyna
        if ((TypPowiadomieniaEnum)powiadomienie.IdTypuPowiadomienia is 
            TypPowiadomieniaEnum.UzytkownikDolaczylDoDruzyny 
            or TypPowiadomieniaEnum.PrzyjecieZaproszeniaDoDruzyny 
            or TypPowiadomieniaEnum.OdrzucenieZaproszeniaDoDruzyny 
            or TypPowiadomieniaEnum.UzytkownikOpuscilDruzyne
        ){
            // nie będzie sytuacji tutaj, że to będzie null, ale aby się nie czepiał kompilator
            var idUzytkownika = powiadomienie.IdPowiazanegoObiektu ?? 1;
            // sprawdzamy, czy taki użytkownik istnieje
            var wynikZnalezieniaUzytkownika = await uzytkownikService.GetUzytkownik(idUzytkownika);
            if (wynikZnalezieniaUzytkownika.StatusCode != 200)
                return ServiceResult<bool>.NotFound(wynikZnalezieniaUzytkownika.Errors[0]);
            
            // sprawdzamy, czy taka drużyna istnieje
            try
            {
                var idDruzyny = powiadomienie.IdDrugiegoPowiazanegoObiektu ?? 1;
                var druzyna = await druzynyRepository.GetDruzyna(idDruzyny);
            }
            catch (NieZnalezionoWBazieException e)
            {
                return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
            }
            
        }
        
        // pierwszym i jedynym powiązanym obiektem jest drużyna
        if ((TypPowiadomieniaEnum)powiadomienie.IdTypuPowiadomienia is 
            TypPowiadomieniaEnum.UsuniecieZDruzyny 
            or TypPowiadomieniaEnum.UzytkownikOpuscilDruzyneBoUsunalKonto
        ){
            try
            { 
                // nie będzie sytuacji tutaj, że to będzie null, ale aby się nie czepiał kompilator
                var idDruzyny = powiadomienie.IdPowiazanegoObiektu ?? 1;
                var druzyna = await druzynyRepository.GetDruzyna(idDruzyny);
            }
            catch (NieZnalezionoWBazieException e)
            {
                return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
            }
        }
        
        // pierwszym powiązanym obiektem jest drużyna, drugim jest miejsce
        if ((TypPowiadomieniaEnum)powiadomienie.IdTypuPowiadomienia is TypPowiadomieniaEnum.ZaproszenieDoDruzyny)
        {
            try
            {    
                // sprawdzamy, czy taka drużyna istnieje
                var idDruzyny = powiadomienie.IdPowiazanegoObiektu ?? 1;
                var druzyna = await druzynyRepository.GetDruzyna(idDruzyny);
                
                // sprawdzamy, czy takie miejsce istnieje
                var idMiejsca = powiadomienie.IdDrugiegoPowiazanegoObiektu ?? 1;
                var wynikZnalezieniaMiejsca = await profilService.GetProfil(idDruzyny);
            }
            catch (NieZnalezionoWBazieException e)
            {
                return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
            }
        }
        
        // jak tu dochodzimy, wszystko jest git
        await powiadomienieRepository.CreatePowiadomienie(powiadomienie);
        // usuwamy powiadomienia przekraczające limit
        await UsunNadmiarowePowiadomieniaUzytkownika(powiadomienie.IdUzytkownika);
        return ServiceResult<bool>.NoContent(true);
        
    }

    public async Task<ServiceResult<bool>> DeletePowiadomieniaZwiazaneZUzytkownikiem(int idUzytkownika)
    {
        if(idUzytkownika < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id użytkownika: " + idUzytkownika));
        // sprawdzamy, czy taki użytkownik istnieje
        
        var uzytkownik = await uzytkownikService.GetUzytkownik(idUzytkownika);
        if (uzytkownik.StatusCode != 200)
            return ServiceResult<bool>.NotFound(uzytkownik.Errors[0]);
        
        // jak tu dochodzimy, wszystko jest git
        return ServiceResult<bool>.NoContent(await powiadomienieRepository.DeletePowiadomieniaZwiazaneZUzytkownikiem(idUzytkownika));
    }

    public async Task<ServiceResult<bool>> UsunPowiadomieniaZwiazaneZDruzyna(int idDruzyny)
    {
        if(idDruzyny < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id drużyny: " + idDruzyny));
        
        return ServiceResult<bool>.NoContent(await powiadomienieRepository.UsunPowiadomieniaZwiazaneZDruzyna(idDruzyny));
    }

    public async Task<ServiceResult<bool>> DeletePowiadomieniaDanegoTypuPowiazaneZObiektami(int? idUzytkownika, int idTypu, int idPowiazanegoObiektu, int? idDrugiegoPowiazanegoObiektu)
    {
        if(idTypu < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id typu powiadomienia: " + idTypu));
        if(idPowiazanegoObiektu < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id powiązanego obiektu: " + idPowiazanegoObiektu));
        if(idDrugiegoPowiazanegoObiektu < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id powiązanego obiektu: " + idPowiazanegoObiektu));
        if (!Enum.IsDefined(typeof(TypPowiadomieniaEnum), idTypu)) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowy typ powiadomienia: " + idTypu));
        
        return ServiceResult<bool>.NoContent(await powiadomienieRepository.DeletePowiadomieniaDanegoTypuPowiazaneZObiektami(idUzytkownika, idTypu, idPowiazanegoObiektu, idDrugiegoPowiazanegoObiektu));
    }
    
    public async Task<bool> UsunNadmiarowePowiadomieniaUzytkownika(int idUzytkownika)
    {
        var powiadomieniaDoUsuniecia = await powiadomienieRepository.PodajPowiadomieniaUzytkownikaPrzekraczajaceLimit(idUzytkownika);
        if (powiadomieniaDoUsuniecia == null || powiadomieniaDoUsuniecia.Count == 0) return true;
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

        // najpierw zmieniamy login na id, potem wywołujemy zapraszanie po id
        public async Task<ServiceResult<bool>> WyslijZaproszenieDoZnajomychPoLoginie(int idZapraszajacego, string loginZaproszonego)
        {
            try
            {
                // filtrujemy, czy podano login
                if (loginZaproszonego.IsNullOrEmpty())
                    return ServiceResult<bool>.BadRequest(
                        new ErrorItem("Nie podano loginu użytkownika, któremu wysyłasz zaproszenie"));

                // pobieramy zapraszanego użytkownika
                var zapraszanyUzytkownikRes = await uzytkownikService.GetUzytkownik(loginZaproszonego);
                
                if (!zapraszanyUzytkownikRes.Succeeded) return ServiceResult<bool>.Fail(zapraszanyUzytkownikRes.StatusCode, zapraszanyUzytkownikRes.Errors);
                
                var idZapraszanego = zapraszanyUzytkownikRes.Value.Id;
                
                // jest git
                return await WyslijZaproszenieDoZnajomychPoId(idZapraszajacego, idZapraszanego);
            }
            catch (NieZnalezionoWBazieException e)
            {
                return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
            }
        }

        public async Task<ServiceResult<bool>> WyslijZaproszenieDoZnajomychPoId(int idZapraszajacego,
            int idZapraszanego)
        {
            if(idZapraszanego == idZapraszajacego)
                    return ServiceResult<bool>.BadRequest(
                        new ErrorItem("Nie możesz wysłać zaproszenia do samego siebie"));
                
                // szukamy, czy zaproszony użytkownik ma już takie zaproszenie
                var powiadomieniaZaproszonego = await powiadomienieRepository.GetPowiadomieniaUzytkownika(idZapraszanego);
                if (powiadomieniaZaproszonego
                    .Any(p => 
                              (TypPowiadomieniaEnum)p.TypPowiadomieniaId == TypPowiadomieniaEnum.ZaproszenieDoZnajomych 
                              && p.PowiazanyObiektId == idZapraszajacego)
                ) { 
                    return ServiceResult<bool>.Conflict(
                        new ErrorItem("Użytkownik o id " + idZapraszanego + " ma już wysłane zaproszenie od Ciebie"));
                }
            
                // sprawdzamy, czy już są znajomymi                         
                if (await znajomiRepository.CzyJestZnajomosc(idZapraszanego, idZapraszajacego))
                {
                    return ServiceResult<bool>.Conflict(
                        new ErrorItem("Użytkownik o id " + idZapraszanego + " jest już Twoim znajomym"));
                }
                
                // sprawdzamy, czy zapraszany jest adminem
                var czyAdminRes = await uzytkownikService.CzyUzytkownikJestAdminem(idZapraszanego);
                if (czyAdminRes.Succeeded && czyAdminRes.Value)
                {
                    return ServiceResult<bool>.Forbidden(
                        new ErrorItem("Podany użytkownik nie może otrzymywać zaproszeń do znajomych"));
                }
                
                // sprawdzamy, czy zapraszajacy nie ma już maksymalnej liczby znajomych
                var znajomiZapraszajacego = await znajomiService.GetZnajomosciUzytkownika(idZapraszajacego);
                if (!znajomiZapraszajacego.Succeeded)
                    return ServiceResult<bool>.BadRequest(znajomiZapraszajacego.Errors[0]);
                if (znajomiZapraszajacego.Value != null && znajomiZapraszajacego.Value.Count >= ZnajomiService.MaxLiczbaZnajomych)
                {
                    return ServiceResult<bool>.Conflict(
                        new ErrorItem("Masz już maksymalną liczbę znajomych i nie możesz wysłać więcej zaproszeń"));
                }
                
                // sprawdzamy, czy zapraszany nie ma już maksymalnej liczby znajomych
                var znajomiZapraszanego = await znajomiService.GetZnajomosciUzytkownika(idZapraszanego);
                if (!znajomiZapraszanego.Succeeded)
                    return ServiceResult<bool>.BadRequest(znajomiZapraszanego.Errors[0]);
                if (znajomiZapraszanego.Value != null && znajomiZapraszanego.Value.Count >= ZnajomiService.MaxLiczbaZnajomych)
                {
                    return ServiceResult<bool>.Conflict(
                        new ErrorItem("Użytkownik o id " + idZapraszanego +
                                      " ma już maksymalną liczbę znajomych i nie może przyjąć więcej zaproszeń"));
                }
                
                // pobieramy profil zapraszającego, aby mieć jego pseudonim do powiadomienia
                var wynikSzukaniaPseudonimuZapraszajacego = await profilService.GetProfil(idZapraszajacego);
                if (wynikSzukaniaPseudonimuZapraszajacego.StatusCode != 200 || wynikSzukaniaPseudonimuZapraszajacego.Value == null)
                    return ServiceResult<bool>.NotFound(
                        new ErrorItem("Nie znaleziono profilu użytkownika o id " + idZapraszanego));
                
                var dto = new PowiadomienieCreateDto(
                    (int)TypPowiadomieniaEnum.ZaproszenieDoZnajomych, 
                    idZapraszanego, 
                    idZapraszajacego, // powiadomienie idzie do zapraszanego użytkownika, powiązany jest wysyłający
                    wynikSzukaniaPseudonimuZapraszajacego.Value.Pseudonim, 
                    null,
                    null,
                    null);

                // jest git
                return ServiceResult<bool>.NoContent(await powiadomienieRepository.CreatePowiadomienie(dto));
        }

        public async Task<ServiceResult<bool>> WyslijPowiadomienieODolaczeniuDoDruzyny(int idDolaczajacego, int idKapitana, int idDruzyny, string nazwaDruzyny, string? nazwaRoli)
        {
            if(idDolaczajacego <=0) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id użytkownika dołączającego: " + idDolaczajacego));
            if(idKapitana <=0) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id kapitana: " + idKapitana));
            if(idDruzyny <=0) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id drużyny: " + idDruzyny));
            
            // pobieramy profil dolaczajacego, aby mieć jego pseudonim do powiadomienia
            var profilDolaczajacegoRes = await profilService.GetProfil(idDolaczajacego);
            if (profilDolaczajacegoRes.StatusCode != 200 || profilDolaczajacegoRes.Value == null)
                return ServiceResult<bool>.Fail(profilDolaczajacegoRes.StatusCode, profilDolaczajacegoRes.Errors);
            
            var dto = new PowiadomienieCreateDto(
                (int)TypPowiadomieniaEnum.UzytkownikDolaczylDoDruzyny,
                idKapitana, // powiadomienie idzie do kapitana
                idDolaczajacego, // powiązany jest dołączający
                profilDolaczajacegoRes.Value.Pseudonim,
                idDruzyny, // powiązana jest drużyna, do której dołączono
                nazwaDruzyny,
                nazwaRoli
            );

            // jest git
            return await CreatePowiadomienie(dto);
        }
        
        // zostałeś zaproszony do drużyny X na rolę Y .
        public async Task<ServiceResult<bool>> WyslijZaproszenieNaMiejsceWDruzynie(int idZapraszanego, int idDruzyny, string nazwaDruzyny, int idMiejsca, string? nazwaRoli)
        {
            if(idMiejsca <=0) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id miejsca: " + idMiejsca));
            if(idZapraszanego <=0) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id uzytkownika zapraszanego: " + idZapraszanego));
            if(idDruzyny <=0) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id drużyny: " + idDruzyny));
            
            
            // sprawdzamy, czy zapraszany jest adminem
            var czyAdminRes = await uzytkownikService.CzyUzytkownikJestAdminem(idZapraszanego);
            if (czyAdminRes.Succeeded && czyAdminRes.Value)
            {
                return ServiceResult<bool>.Forbidden(
                    new ErrorItem("Podany użytkownik nie może otrzymywać zaproszeń do drużyny"));
            }
            
            // usuwamy stare zaproszenia na dane miejsce, aby nie było duplikatów
            await powiadomienieRepository.DeletePowiadomieniaDanegoTypuPowiazaneZObiektami(null, (int)TypPowiadomieniaEnum.ZaproszenieDoDruzyny, idDruzyny, idMiejsca);
            
            // usuwamy inne zaproszenia danego użytkownika do danej drużyny, bo je nadpisujemy
            await DeletePowiadomieniaDanegoTypuPowiazaneZObiektami(idZapraszanego, (int)TypPowiadomieniaEnum.ZaproszenieDoDruzyny, idDruzyny, null);
            
            var dto = new PowiadomienieCreateDto(
                (int)TypPowiadomieniaEnum.ZaproszenieDoDruzyny,
                idZapraszanego, // powiadomienie idzie do zapraszanego
                idDruzyny, // powiązana jest drużyna, której kapitan zaprasza
                nazwaDruzyny,
                idMiejsca, // powiązane jest miejsce, na które zapraszany jest użytkownik
                null, // nazwa miejsca jest zbędna, bo i tak nie będzie wyświetlana w powiadomieniu
                nazwaRoli
            );

            // jest git
            return await CreatePowiadomienie(dto);
        }
        
        public async Task<ServiceResult<bool>> WyslijPowiadomienieOUsunieciuZDruzyny(int idUsuwanego, int idDruzyny, string nazwaDruzyny)
        {
            if(idUsuwanego <=0) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id użytkownika usuwanego: " + idUsuwanego));
            if(idDruzyny <=0) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id drużyny: " + idDruzyny));
            
            // pobieramy profil usuwanego, aby mieć jego pseudonim do powiadomienia
            var profilUsuwanegoRes = await profilService.GetProfil(idUsuwanego);
            if (profilUsuwanegoRes.StatusCode != 200 || profilUsuwanegoRes.Value == null)
                return ServiceResult<bool>.Fail(profilUsuwanegoRes.StatusCode, profilUsuwanegoRes.Errors);
            
            var dto = new PowiadomienieCreateDto(
                (int)TypPowiadomieniaEnum.UsuniecieZDruzyny,
                idUsuwanego, // powiadomienie idzie do usuwanego użytkownika
                idDruzyny, // powiązana jest drużyna, z której usunięto
                nazwaDruzyny,
                null, 
                null,
                null
            );

            // jest git
            return await CreatePowiadomienie(dto);
        }

        public async Task<ServiceResult<bool>> WyslijPowiadomienieOWyjsciuZDruzyny(int idKapitana, int? idOpuszczajacego, int idDruzyny, string nazwaDruzyny, string? nazwaRoli, bool czyPrzyUsuwaniuKonta)
        {
            
            if(idKapitana <=0) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id kapitana: " + idKapitana));
            if(idOpuszczajacego <=0) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id użytkownika opuszczającego: " + idOpuszczajacego));
            if(idDruzyny <=0) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id drużyny: " + idDruzyny));

            if(!czyPrzyUsuwaniuKonta){
                // pobieramy profil opuszczającego, aby mieć jego pseudonim do powiadomienia
                var profilOpuszczajacegoRes = await profilService.GetProfil(idOpuszczajacego ?? 1); // nie będzie sytuacji tutaj, że to będzie null, ale aby się nie czepiał kompilator
                if (profilOpuszczajacegoRes.StatusCode != 200 || profilOpuszczajacegoRes.Value == null)
                    return ServiceResult<bool>.Fail(profilOpuszczajacegoRes.StatusCode, profilOpuszczajacegoRes.Errors);

                var dto = new PowiadomienieCreateDto(
                    (int)TypPowiadomieniaEnum.UzytkownikOpuscilDruzyne,
                    idKapitana, // powiadomienie idzie do kapitana
                    idOpuszczajacego, // powiązany jest opuszczający użytkownik
                    profilOpuszczajacegoRes.Value.Pseudonim,
                    idDruzyny, // powiązana jest drużyna, którą opuszczono
                    nazwaDruzyny,
                    nazwaRoli
                );
                // jest git
                return await CreatePowiadomienie(dto);
            }
            else
            {
                var dto = new PowiadomienieCreateDto(
                    (int)TypPowiadomieniaEnum.UzytkownikOpuscilDruzyneBoUsunalKonto,
                    idKapitana, // powiadomienie idzie do kapitana
                    idDruzyny, // powiązana jest drużyna, którą opuszczono
                    nazwaDruzyny,
                    null, 
                    null,
                    nazwaRoli
                );
                // jest git
                return await CreatePowiadomienie(dto);
            }
            
        }
        
        public async Task<ServiceResult<bool>> WyslijPowiadomienieORozwiazaniuDruzyny(int idOdbiorcy, string nazwaDruzyny)
        {
            if(idOdbiorcy <=0) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id odbiorcy: " + idOdbiorcy));
            
            var dto = new PowiadomienieCreateDto(
                (int)TypPowiadomieniaEnum.DruzynaZostalaRozwiazana,
                idOdbiorcy, 
                null,
                nazwaDruzyny,
                null,
                null,
                null
            );
            
            return await CreatePowiadomienie(dto);
        }
        
        public async Task<ServiceResult<bool>> WyslijPowiadomienieOUsunieciuDruzynyPrzezAdmina(int idOdbiorcy, string nazwaDruzyny)
        {
            if(idOdbiorcy <=0) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id odbiorcy: " + idOdbiorcy));
            
            var dto = new PowiadomienieCreateDto(
                (int)TypPowiadomieniaEnum.DruzynaZostalaUsunietaPrzezAdmina,
                idOdbiorcy, 
                null,
                nazwaDruzyny,
                null,
                null,
                null
            );
            
            return await CreatePowiadomienie(dto);
        }
}