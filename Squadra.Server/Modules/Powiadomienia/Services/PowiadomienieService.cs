using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Squadra.Server.DTO.Powiadomienie;
using Squadra.Server.Exceptions;
using Squadra.Server.Models;
using Squadra.Server.Repositories;

namespace Squadra.Server.Services;

public class PowiadomienieService(IPowiadomienieRepository powiadomienieRepository,
    UserManager<Uzytkownik> userManager,
    IUzytkownikService uzytkownikService,
    IZnajomiService znajomiService,
    IZnajomiRepository znajomiRepository,
    IProfilService profilService
    ) : IPowiadomienieService
{
    public async Task<ServiceResult<PowiadomienieDto>> GetPowiadomienie(int id, ClaimsPrincipal user) {
        var powiadomienie = await powiadomienieRepository.GetPowiadomienie(id);
        var uzytkownik = await userManager.GetUserAsync(user);
        if(uzytkownik == null) return ServiceResult<PowiadomienieDto>.Unauthorized(new ErrorItem("Nie jesteś zalogowany"));
        if (powiadomienie.UzytkownikId != uzytkownik.Id)
        {
            return ServiceResult<PowiadomienieDto>.Forbidden(new ErrorItem("Nie możesz pobrać powiadomienia innego użytkownika"));
        } 
        return ServiceResult<PowiadomienieDto>.Ok(powiadomienie);
    }

    public async Task<ServiceResult<ICollection<PowiadomienieDto>>> GetPowiadomieniaUzytkownika(int idUzytkownika)
    {
        // czy to dobry użytkownik sprawdzamy już w controllerze, bo mamy od razu id
        return ServiceResult<ICollection<PowiadomienieDto>>.Ok(await powiadomienieRepository.GetPowiadomieniaUzytkownika(idUzytkownika));
    }

    public async Task<ServiceResult<bool>> CreatePowiadomienie(PowiadomienieCreateDto powiadomienie)
    {
        // okolicznościami tworzenia powiadomienia zajmują się inne klasy, tutaj tylko tworzymy
        if (powiadomienie.IdTypuPowiadomienia < 1)
            return ServiceResult<bool>.NotFound(new ErrorItem("Typ powiadomienia o id " +
                                                              powiadomienie.IdTypuPowiadomienia + " nie istnieje"));
        if (powiadomienie.IdPowiazanegoObiektu < 1)
            return ServiceResult<bool>.NotFound(new ErrorItem("Obiekt o id " + powiadomienie.IdPowiazanegoObiektu +
                                                              " nie istnieje w żadnym typie obiektów"));

        // sprawdzamy, czy odnosi się do istniejącego obiektu

        // odnośnie znajomych (nowe zaproszenie, zaakceptowano zaproszenie, odrzucono zaproszenie, usunięto ze znajomych)
        if (powiadomienie.IdTypuPowiadomienia is 2 or 3 or 4 or 5)
        {
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

        // robimy rozpatrzenie odpowiedzi na powiadomienie. Jeżeli jest to drugie, to reagujemy inaczej niż w przypadku reszty (na ten moment), bo wymagana jest akcja
        // jeżeli to typ "zaproszenie do znajomych", czyZaakceptowane nie jest null

        public async Task<ServiceResult<bool>> RozpatrzPowiadomienie(int id, bool? czyZaakceptowane, ClaimsPrincipal user)
        {
            try
            {
                var powiadomienie = await powiadomienieRepository.GetPowiadomienie(id);
                /*
                    int Id,
                    int IdTypuPowiadomienia,
                    int UzytkownikId,
                    int? IdPowiazanegoObiektu,
                    string? NazwaPowiazanegoObiektu, // pseudonim dla uzytkownika i nazwa dla gildii
                    string? Tresc,
                    DateTime DataWyslania
                 */
                var uzytkownik = await userManager.GetUserAsync(user);
                if(uzytkownik == null) return ServiceResult<bool>.Unauthorized(new ErrorItem("Nie jesteś zalogowany"));
                if (powiadomienie.UzytkownikId != uzytkownik.Id)
                {
                    return ServiceResult<bool>.Forbidden(new ErrorItem("Nie możesz rozpatrzyć powiadomienia innego użytkownika"));
                }
                // jak tu doszliśmy, wszystko jest git, chyba że nie podano powiązanego obiektu w konkretnych typach
        
                // zaproszenie do znajomych
                if (powiadomienie.IdTypuPowiadomienia == 2)
                {
                
                    if(powiadomienie.IdPowiazanegoObiektu == null) return ServiceResult<bool>.BadRequest(new ErrorItem("Nie podano użytkownika, którego zaproszenie akceptujesz"));
                
                    // pobieramy naszą nazwę użytkownika, aby była w powiadomieniu dla drugiej strony
                    var wynikZnalezieniaProfilu = await profilService.GetProfil(uzytkownik.Id);
                    if(wynikZnalezieniaProfilu.StatusCode != 200) return ServiceResult<bool>.NotFound(wynikZnalezieniaProfilu.Errors[0]);
                    if(wynikZnalezieniaProfilu.Value == null) return ServiceResult<bool>.NotFound(new ErrorItem("Nie znaleziono profilu użytkownika o id " + powiadomienie.IdPowiazanegoObiektu.Value));
                
                    switch (czyZaakceptowane)
                    {
                        case true:
                        {
                            var result = await znajomiService.CreateZnajomosc(powiadomienie.UzytkownikId, powiadomienie.IdPowiazanegoObiektu ?? 1); // aby się kompilator nie czepiał
                            // coś poszło nie tak
                            if (result.StatusCode != 201) return result;
                        
                        
                            // wszystko git
                            // wysyłamy uzytkownikowi, który jest powiązany, że jego zaproszenie zostało zaakceptowane
                            await powiadomienieRepository.CreatePowiadomienie(new PowiadomienieCreateDto(
                                // zaakceptowano zaproszenie
                                3,
                                // wysyłamy to użytkownikowi, którego to zaproszenie dotyczy
                                powiadomienie.IdPowiazanegoObiektu ?? 1, // już null odfiltrowaliśmy, ale aby się nie czepiał kompilator
                                // powiązany jest użytkownik, który zaakceptował
                                uzytkownik.Id,
                                wynikZnalezieniaProfilu.Value.Pseudonim,
                                // treść zostanie sklejona na miejscu
                                null
                            ));
                            break;
                        }
                        case false:
                        {
                            // wysyłamy uzytkownikowi, który jest powiązany, że jego zaproszenie zostało odrzucone
                            await powiadomienieRepository.CreatePowiadomienie(new PowiadomienieCreateDto(
                                // odrzucono zaproszenie
                                4,
                                // wysyłamy to użytkownikowi, którego to zaproszenie dotyczy
                                powiadomienie.IdPowiazanegoObiektu ?? 1, // już to odfiltrowaliśmy, ale aby się nie czepiał kompilator
                                // powiązany jest użytkownik, który odrzucił
                                uzytkownik.Id,
                                wynikZnalezieniaProfilu.Value.Pseudonim,
                                // treść zostanie sklejona na miejscu
                                null
                            ));
                            break;
                        }
                        default:
                        {
                            return ServiceResult<bool>.BadRequest(new ErrorItem("Nie podano, czy zaakceptowano zaproszenie"));
                        }
                    }
                }
            
                // jak tu dochodzimy, wszystko zostało pomyślnie rozpatrzone i usuwamy
                await powiadomienieRepository.DeletePowiadomienie(powiadomienie.Id);
                return ServiceResult<bool>.NoContent(true);
            }
            catch (NieZnalezionoWBazieException e)
            {
                return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
            }
        }

        // najpierw zmieniamy login na id, potem sprawdzamy, czy już nie są znajomymi, na końcu tworzymy zaproszenie
        public async Task<ServiceResult<bool>> WyslijZaproszenieDoZnajomych(int idZapraszajacego, string loginZaproszonego)
        {
            try
            {
                // filtrujemy, czy podano login
                if (loginZaproszonego.IsNullOrEmpty())
                    return ServiceResult<bool>.NotFound(
                        new ErrorItem("Nie podano loginu użytkownika, któremu wysyłasz zaproszenie"));

                // pobieramy zapraszanego użytkownika
                var zapraszanyUzytkownik = await userManager.FindByNameAsync(loginZaproszonego);
                
                if (zapraszanyUzytkownik == null)
                    return ServiceResult<bool>.NotFound(
                        new ErrorItem("Użytkownik o loginie " + loginZaproszonego + " nie istnieje"));
                
                var idZapraszanego = zapraszanyUzytkownik.Id;
                if(idZapraszanego == idZapraszajacego)
                    return ServiceResult<bool>.BadRequest(
                        new ErrorItem("Nie możesz wysłać zaproszenia do samego siebie"));
                
                // szukamy, czy zaproszony użytkownik ma już takie zaproszenie
                var powiadomieniaZaproszonego = await powiadomienieRepository.GetPowiadomieniaUzytkownika(idZapraszanego);
                if (powiadomieniaZaproszonego.Any(p => p.IdTypuPowiadomienia == 2 && p.IdPowiazanegoObiektu == idZapraszajacego))
                { 
                    return ServiceResult<bool>.Conflict(
                        new ErrorItem("Użytkownik o loginie " + loginZaproszonego + " ma już wysłane zaproszenie od Ciebie"));
                }
            
                // sprawdzamy, czy już są znajomymi                         
                if (await znajomiRepository.CzyJestZnajomosc(idZapraszanego, idZapraszajacego))
                {
                    return ServiceResult<bool>.Conflict(
                        new ErrorItem("Użytkownik o id " + idZapraszanego + " jest już Twoim znajomym"));
                }
                
                // sprawdzamy, czy zapraszajacy nie ma już maksymalnej liczby znajomych
                var znajomiZapraszajacego = await znajomiService.GetZnajomiUzytkownika(idZapraszajacego);
                if (znajomiZapraszajacego.Value != null && znajomiZapraszajacego.Value.Count >= ZnajomiService.MaxLiczbaZnajomych)
                {
                    return ServiceResult<bool>.Conflict(
                        new ErrorItem("Masz już maksymalną liczbę znajomych i nie możesz wysłać więcej zaproszeń"));
                }
                
                // sprawdzamy, czy zapraszany nie ma już maksymalnej liczby znajomych
                var znajomiZapraszanego = await znajomiService.GetZnajomiUzytkownika(idZapraszanego);
                if (znajomiZapraszanego.Value != null && znajomiZapraszanego.Value.Count >= ZnajomiService.MaxLiczbaZnajomych)
                {
                    return ServiceResult<bool>.Conflict(
                        new ErrorItem("Użytkownik o loginie " + loginZaproszonego +
                                      " ma już maksymalną liczbę znajomych i nie może przyjąć więcej zaproszeń"));
                }
                
                // pobieramy profil zapraszającego, aby mieć jego pseudonim do powiadomienia
                var wynikSzukaniaPseudonimuZapraszajacego = await profilService.GetProfil(idZapraszajacego);
                if (wynikSzukaniaPseudonimuZapraszajacego.StatusCode != 200 || wynikSzukaniaPseudonimuZapraszajacego.Value == null)
                    return ServiceResult<bool>.NotFound(
                        new ErrorItem("Nie znaleziono profilu użytkownika o loginie " + loginZaproszonego));
                
                var dto = new PowiadomienieCreateDto(
                    2, 
                    idZapraszanego, 
                    idZapraszajacego, // powiadomienie idzie do zapraszanego użytkownika, powiązany jest wysyłający
                    wynikSzukaniaPseudonimuZapraszajacego.Value.Pseudonim, 
                    null);

                // jest git
                return ServiceResult<bool>.NoContent(await powiadomienieRepository.CreatePowiadomienie(dto));
            }
            catch (NieZnalezionoWBazieException e)
            {
                return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
            }
        }
}