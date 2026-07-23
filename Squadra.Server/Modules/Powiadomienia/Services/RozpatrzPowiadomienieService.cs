using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Drużyny.Services;
using Squadra.Server.Modules.Powiadomienia.DTO;
using Squadra.Server.Modules.Powiadomienia.Enums;
using Squadra.Server.Modules.Powiadomienia.Repositories;
using Squadra.Server.Modules.Profile.Services;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Statystyki.Services;
using Squadra.Server.Modules.Znajomosci.Services;

namespace Squadra.Server.Modules.Powiadomienia.Services;

public class RozpatrzPowiadomienieService(IPowiadomieniaRepository powiadomieniaRepository,
    IZnajomosciService znajomosciService,
    IProfileService profileService,
    IDruzynyService druzynyService,
    IStatystykiService statystykiService
) : IRozpatrzPowiadomienieService
{
    // robimy rozpatrzenie odpowiedzi na powiadomienie. Jeżeli to zaproszenie do znajomych lub drużyny, to musimy wiedzieć, czy zostało zaakceptowane, czy odrzucone, bo w zależności od tego trzeba wykonać różne akcje.
    // W innych przypadkach ten parametr jest ignorowany
    public async Task<ServiceResult<bool>> RozpatrzPowiadomienie(int id, bool? czyZaakceptowane, int idUzytkownika)
    {
        try
        {
            var powiadomienie = await powiadomieniaRepository.GetPowiadomienie(id);
            /*
                int Id,
                int IdTypuPowiadomienia,
                int UzytkownikId,
                int? IdPowiazanegoObiektu, // użytkownik lub drużyna
                string? NazwaPowiazanegoObiektu,
                int? IdDrugiegoPowiazanegoObiektu, // drużyna lub miejsce
                string? NazwaDrugiegoPowiazanegoObiektu,
                string? Tresc,
                DateTime DataWyslania
             */
            
            if (powiadomienie.UzytkownikId != idUzytkownika)
            {
                return ServiceResult<bool>.Forbidden(new ErrorItem("Nie możesz rozpatrzyć powiadomienia innego użytkownika"));
            }
            // jak tu doszliśmy, wszystko jest w porządku, chyba że nie podano powiązanego obiektu w konkretnych typach
    
            // zaproszenie do znajomych
            if ((TypPowiadomieniaEnum)powiadomienie.TypPowiadomieniaId == TypPowiadomieniaEnum.ZaproszenieDoZnajomych)
            {
            
                if(powiadomienie.PowiazanyObiektId == null)
                {
                    // skoro jest błędne powiadomienie, to usuwamy je, bo nic innego nie możemy zrobić, a lepiej, żeby go nie było, niż żeby ciągle był i ktoś próbował na niego reagować
                    return await UsunRozpatrywanePowiadomienie(powiadomienie.Id);
                }
            
                // pobieramy naszą nazwę użytkownika, aby była w powiadomieniu dla drugiej strony
                var wynikZnalezieniaProfilu = await profileService.GetProfil(idUzytkownika);
                if(wynikZnalezieniaProfilu.StatusCode != 200)
                {
                    // skoro jest błędne powiadomienie, to usuwamy je, bo nic innego nie możemy zrobić, a lepiej, żeby go nie było, niż żeby ciągle był i ktoś próbował na niego reagować
                    await UsunRozpatrywanePowiadomienie(powiadomienie.Id);
                    return ServiceResult<bool>.NotFound(wynikZnalezieniaProfilu.Errors[0]);
                }
                if(wynikZnalezieniaProfilu.Value == null)
                {
                    // skoro jest błędne powiadomienie, to usuwamy je, bo nic innego nie możemy zrobić, a lepiej, żeby go nie było, niż żeby ciągle był i ktoś próbował na niego reagować
                    await UsunRozpatrywanePowiadomienie(powiadomienie.Id);
                    return ServiceResult<bool>.NotFound(new ErrorItem("Nie znaleziono profilu użytkownika o id " + powiadomienie.PowiazanyObiektId));
                }
            
                switch (czyZaakceptowane)
                {
                    case true:
                    {
                        var result = await znajomosciService.CreateZnajomosc(powiadomienie.UzytkownikId, powiadomienie.PowiazanyObiektId ?? 1); // "??" aby się kompilator nie czepiał
                        // coś poszło nie tak
                        if (result.StatusCode != 201) return result;
                        
                        // wszystko w porządku
                        // wysyłamy uzytkownikowi, który jest powiązany, że jego zaproszenie zostało zaakceptowane
                        await powiadomieniaRepository.CreatePowiadomienie(new PowiadomienieCreateDto(
                            // zaakceptowano zaproszenie
                            (int)TypPowiadomieniaEnum.PrzyjecieZaproszeniaDoZnajomych,
                            // wysyłamy to użytkownikowi, którego to zaproszenie dotyczy
                            powiadomienie.PowiazanyObiektId ?? 1, // już null odfiltrowaliśmy, ale aby się nie czepiał kompilator
                            // powiązany jest użytkownik, który zaakceptował
                            idUzytkownika,
                            wynikZnalezieniaProfilu.Value.Pseudonim,
                            null,
                            null,
                            // treść zostanie sklejona na miejscu
                            null
                        ));
                        break;
                    }
                    case false:
                    {
                        // wysyłamy uzytkownikowi, który jest powiązany, że jego zaproszenie zostało odrzucone
                        await powiadomieniaRepository.CreatePowiadomienie(new PowiadomienieCreateDto(
                            // odrzucono zaproszenie
                            (int)TypPowiadomieniaEnum.OdrzucenieZaproszeniaDoZnajomych,
                            // wysyłamy to użytkownikowi, którego to zaproszenie dotyczy
                            powiadomienie.PowiazanyObiektId ?? 1, // już to odfiltrowaliśmy, ale aby się nie czepiał kompilator
                            // powiązany jest użytkownik, który odrzucił
                            idUzytkownika,
                            wynikZnalezieniaProfilu.Value.Pseudonim,
                            null,
                            null,
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

            // zaproszenie do drużyny
            if ((TypPowiadomieniaEnum)powiadomienie.TypPowiadomieniaId == TypPowiadomieniaEnum.ZaproszenieDoDruzyny)
            {
                if(powiadomienie.PowiazanyObiektId == null || powiadomienie.DrugiPowiazanyObiektId == null)
                {
                    // skoro jest błędne powiadomienie, to usuwamy je, bo nic innego nie możemy zrobić, a lepiej, żeby go nie było, niż żeby ciągle był i ktoś próbował na niego reagować
                    return await UsunRozpatrywanePowiadomienie(powiadomienie.Id);
                }
                
                // pobieramy naszą nazwę użytkownika, aby była w powiadomieniu dla drugiej strony
                var wynikZnalezieniaProfilu = await profileService.GetProfil(idUzytkownika);
                if(wynikZnalezieniaProfilu.StatusCode != 200)
                {
                    // skoro jest błędne powiadomienie, to usuwamy je, bo nic innego nie możemy zrobić, a lepiej, żeby go nie było, niż żeby ciągle był i ktoś próbował na niego reagować
                    await UsunRozpatrywanePowiadomienie(powiadomienie.Id);
                    return ServiceResult<bool>.NotFound(wynikZnalezieniaProfilu.Errors[0]);
                }
                if(wynikZnalezieniaProfilu.Value == null)
                {
                    // skoro jest błędne powiadomienie, to usuwamy je, bo nic innego nie możemy zrobić, a lepiej, żeby go nie było, niż żeby ciągle był i ktoś próbował na niego reagować
                    await UsunRozpatrywanePowiadomienie(powiadomienie.Id);
                    return ServiceResult<bool>.NotFound(new ErrorItem("Nie znaleziono profilu użytkownika o id " + idUzytkownika));
                }
                
                // pobieramy miejsce w drużynie
                var miejsceRes = await druzynyService.GetMiejsceWDruzynie(powiadomienie.DrugiPowiazanyObiektId ?? 1);
                if (miejsceRes.StatusCode != 200 || miejsceRes.Value == null)
                {
                    // skoro jest błędne powiadomienie, to usuwamy je, bo nic innego nie możemy zrobić, a lepiej, żeby go nie było, niż żeby ciągle był i ktoś próbował na niego reagować
                    return await UsunRozpatrywanePowiadomienie(powiadomienie.Id);
                }
            
                // pobieramy drużynę, do której należy to miejsce
                var druzynaRes = await druzynyService.GetDruzynaMiejsca(powiadomienie.DrugiPowiazanyObiektId ?? 1);
                if (druzynaRes.StatusCode != 200 || druzynaRes.Value == null)
                {
                    return await UsunRozpatrywanePowiadomienie(powiadomienie.Id);
                }

                string? nazwaRoli = null;
                if(miejsceRes.Value.RolaId != null)
                {
                    var rolaRes = await statystykiService.GetRola(miejsceRes.Value.RolaId ?? 1);
                    if(rolaRes.Succeeded && rolaRes.Value != null) nazwaRoli = rolaRes.Value.Nazwa;
                }
                
                switch (czyZaakceptowane)
                {
                    case true:
                    {
                        var result = await druzynyService.DodajUzytkownikaNaMiejsce(powiadomienie.DrugiPowiazanyObiektId ?? 1, powiadomienie.UzytkownikId); 
                        // coś poszło nie tak
                        if (!result.Succeeded) return result;
                        
                        // wszystko w porządku
                        // użytkownik X przyjął Twoje zaproszenie do drużyny Y na rolę Z.
                        await powiadomieniaRepository.CreatePowiadomienie(new PowiadomienieCreateDto(
                            // zaakceptowano zaproszenie
                            (int)TypPowiadomieniaEnum.PrzyjecieZaproszeniaDoDruzyny,
                            // wysyłamy to kapitanowi drużyny, której to zaproszenie dotyczy
                            druzynaRes.Value.KapitanId,
                            // powiązany jest użytkownik, który zaakceptował
                            idUzytkownika,
                            wynikZnalezieniaProfilu.Value.Pseudonim,
                            druzynaRes.Value.Id,
                            druzynaRes.Value.Nazwa,
                            nazwaRoli
                        ));
                        break;
                    }
                    case false:
                    {
                        // użytkownik X odrzucił Twoje zaproszenie do drużyny Y na rolę Z.
                        await powiadomieniaRepository.CreatePowiadomienie(new PowiadomienieCreateDto(
                            // odrzucono zaproszenie
                            (int)TypPowiadomieniaEnum.OdrzucenieZaproszeniaDoDruzyny,
                            // wysyłamy to kapitanowi drużyny, której to zaproszenie dotyczy
                            druzynaRes.Value.KapitanId,
                            // powiązany jest użytkownik, który zaakceptował
                            idUzytkownika,
                            wynikZnalezieniaProfilu.Value.Pseudonim,
                            druzynaRes.Value.Id,
                            druzynaRes.Value.Nazwa,
                            nazwaRoli
                        ));
                        break;
                    }
                    default:
                    {
                        return ServiceResult<bool>.BadRequest(new ErrorItem("Nie podano, czy zaakceptowano zaproszenie"));
                    }
                }
            }

            return await UsunRozpatrywanePowiadomienie(powiadomienie.Id);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    private async Task<ServiceResult<bool>> UsunRozpatrywanePowiadomienie(int id)
    {
        try
        {
            await powiadomieniaRepository.DeletePowiadomienie(id);
        }
        catch (NieZnalezionoWBazieException e)
        {
            // nic nie robimy, bo skoro nie ma powiadomienia, to coś innego je usunęło i tyle
        }
        return ServiceResult<bool>.NoContent(true);
    }
}