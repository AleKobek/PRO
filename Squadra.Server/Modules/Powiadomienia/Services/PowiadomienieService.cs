using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Powiadomienia.DTO;
using Squadra.Server.Modules.Powiadomienia.Enums;
using Squadra.Server.Modules.Powiadomienia.Repositories;
using Squadra.Server.Modules.Profile.Services;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Uzytkownicy.Models;
using Squadra.Server.Modules.Uzytkownicy.Services;
using Squadra.Server.Modules.Znajomosci.Repositories;
using Squadra.Server.Modules.Znajomosci.Services;

namespace Squadra.Server.Modules.Powiadomienia.Services;

public class PowiadomienieService(IPowiadomienieRepository powiadomienieRepository,
    UserManager<Uzytkownik> userManager,
    IUzytkownikService uzytkownikService,
    IZnajomiService znajomiService,
    IZnajomiRepository znajomiRepository,
    IProfilService profilService
    ) : IPowiadomienieService
{
    public async Task<ServiceResult<PowiadomienieDto>> GetPowiadomienie(int id, ClaimsPrincipal user) {
        if(id < 1) return ServiceResult<PowiadomienieDto>.BadRequest(new ErrorItem("Nieprawidłowe id powiadomienia: " + id));
        var powiadomienie = await powiadomienieRepository.GetPowiadomienie(id);
        var uzytkownik = await userManager.GetUserAsync(user);
        if(uzytkownik == null) return ServiceResult<PowiadomienieDto>.Unauthorized(new ErrorItem("Nie jesteś zalogowany"));
        if (powiadomienie.UzytkownikId != uzytkownik.Id)
        {
            return ServiceResult<PowiadomienieDto>.Forbidden(new ErrorItem("Nie możesz pobrać powiadomienia innego użytkownika"));
        }
        
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
        // jak tu dochodzimy, wszystko jest git

        return ServiceResult<bool>.NoContent(await powiadomienieRepository.CreatePowiadomienie(powiadomienie));
        
    }

    public async Task<ServiceResult<bool>> DeletePowiadomieniaUzytkownika(int idUzytkownika)
    {
        if(idUzytkownika < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id użytkownika: " + idUzytkownika));
        // sprawdzamy, czy taki użytkownik istnieje
        
        var uzytkownik = await uzytkownikService.GetUzytkownik(idUzytkownika);
        if (uzytkownik.StatusCode != 200)
            return ServiceResult<bool>.NotFound(uzytkownik.Errors[0]);
        
        // jak tu dochodzimy, wszystko jest git
        return ServiceResult<bool>.NoContent(await powiadomienieRepository.DeletePowiadomieniaUzytkownika(idUzytkownika));
    }

    public async Task<ServiceResult<bool>> DeletePowiadomieniaDanegoTypuPowiazaneZObiektami(int? idUzytkownika, int idTypu, int idPowiazanegoObiektu, int? idDrugiegoPowiazanegoObiektu)
    {
        if(idTypu < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id typu powiadomienia: " + idTypu));
        if(idPowiazanegoObiektu < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id powiązanego obiektu: " + idPowiazanegoObiektu));
        if(idDrugiegoPowiazanegoObiektu < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id powiązanego obiektu: " + idPowiazanegoObiektu));
        if (!Enum.IsDefined(typeof(TypPowiadomieniaEnum), idTypu)) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowy typ powiadomienia: " + idTypu));
        
        return ServiceResult<bool>.NoContent(await powiadomienieRepository.DeletePowiadomieniaDanegoTypuPowiazaneZObiektami(idUzytkownika, idTypu, idPowiazanegoObiektu, idDrugiegoPowiazanegoObiektu));
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
                var zapraszanyUzytkownik = await userManager.FindByNameAsync(loginZaproszonego);
                
                if (zapraszanyUzytkownik == null)
                    return ServiceResult<bool>.NotFound(
                        new ErrorItem("Użytkownik o loginie " + loginZaproszonego + " nie istnieje"));
                
                var idZapraszanego = zapraszanyUzytkownik.Id;
                
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
                
                // sprawdzamy, czy zapraszajacy nie ma już maksymalnej liczby znajomych
                var znajomiZapraszajacego = await znajomiService.GetZnajomiUzytkownika(idZapraszajacego);
                if (!znajomiZapraszajacego.Succeeded)
                    return ServiceResult<bool>.BadRequest(znajomiZapraszajacego.Errors[0]);
                if (znajomiZapraszajacego.Value != null && znajomiZapraszajacego.Value.Count >= ZnajomiService.MaxLiczbaZnajomych)
                {
                    return ServiceResult<bool>.Conflict(
                        new ErrorItem("Masz już maksymalną liczbę znajomych i nie możesz wysłać więcej zaproszeń"));
                }
                
                // sprawdzamy, czy zapraszany nie ma już maksymalnej liczby znajomych
                var znajomiZapraszanego = await znajomiService.GetZnajomiUzytkownika(idZapraszanego);
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

        public async Task<ServiceResult<bool>> WyslijPowiadomienieOWyjsciuZDruzyny(int idKapitana, int idOpuszczajacego, int idDruzyny, string nazwaDruzyny, string? nazwaRoli)
        {
            
            if(idKapitana <=0) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id kapitana: " + idKapitana));
            if(idOpuszczajacego <=0) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id użytkownika opuszczającego: " + idOpuszczajacego));
            if(idDruzyny <=0) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowe id drużyny: " + idDruzyny));
            
            // pobieramy profil opuszczającego, aby mieć jego pseudonim do powiadomienia
            var profilOpuszczajacegoRes = await profilService.GetProfil(idOpuszczajacego);
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
}