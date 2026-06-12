using Squadra.Server.Exceptions;
using Squadra.Server.Modules.BibliotekaGier.Services;
using Squadra.Server.Modules.Drużyny.DTO;
using Squadra.Server.Modules.Drużyny.Repositories;
using Squadra.Server.Modules.Platformy.Services;
using Squadra.Server.Modules.Powiadomienia.Enums;
using Squadra.Server.Modules.Powiadomienia.Services;
using Squadra.Server.Modules.Profile.DTO.Profil;
using Squadra.Server.Modules.Profile.Services;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Statystyki.Services;
using Squadra.Server.Modules.Uzytkownicy.Services;
using Squadra.Server.Modules.WspieraneGry.DTO;
using Squadra.Server.Modules.WspieraneGry.Services;

namespace Squadra.Server.Modules.Drużyny.Services;

public class DruzynyService(
    IDruzynyRepository druzynyRepository,
    IWspieranaGraService wspieranaGraService,
    IUzytkownikService uzytkownikService,
    IProfilService profilService,
    IJezykService jezykService,
    IStopienBieglosciJezykaService stopienBieglosciJezykaService,
    IStatystykiService statystykiService,
    IPlatformaService platformaService,
    IBibliotekaGierService bibliotekaGierService,
    IPowiadomienieService powiadomienieService
    ) : IDruzynyService
{
    
    public static readonly int LiczbaDruzynNaStroneNaStart = 20;


    public async Task<ServiceResult<DruzynaDto>> GetDruzyna(int id)
    {
        try
        {
            var druzyna = await druzynyRepository.GetDruzyna(id);
            return ServiceResult<DruzynaDto>.Ok(new DruzynaDto(
                druzyna.Id,
                druzyna.Nazwa,
                druzyna.GraId,
                druzyna.KapitanId,
                druzyna.CzyPubliczna,
                druzyna.Opis,
                druzyna.NastrojRozgrywkiId,
                druzyna.WymaganyJezykId,
                druzyna.WymaganyStopienBieglosciJezykaId,
                druzyna.PlatformaId,
                druzyna.CzyZintegrowano
            ));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<DruzynaDto>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<DruzynaDto>> GetDruzynaMiejsca(int idMiejsca)
    {
        if(idMiejsca < 1) return ServiceResult<DruzynaDto>.BadRequest(new ErrorItem("Nieprawidłowe id miejsca w drużynie: " + idMiejsca));
        try
        {
            var druzyna = await druzynyRepository.GetDruzynaMiejsca(idMiejsca);
            return ServiceResult<DruzynaDto>.Ok(new DruzynaDto(
                druzyna.Id,
                druzyna.Nazwa,
                druzyna.GraId,
                druzyna.KapitanId,
                druzyna.CzyPubliczna,
                druzyna.Opis,
                druzyna.NastrojRozgrywkiId,
                druzyna.WymaganyJezykId,
                druzyna.WymaganyStopienBieglosciJezykaId,
                druzyna.PlatformaId,
                druzyna.CzyZintegrowano
            ));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<DruzynaDto>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<MiejsceWDruzynieDto>> GetMiejsceWDruzynie(int idMiejscaWDruzynie)
    {
        if (idMiejscaWDruzynie <= 0)
            return ServiceResult<MiejsceWDruzynieDto>.BadRequest(
                new ErrorItem("Id miejsca w drużynie musi być większe od 0"));
        try
        {
            var miejsce = await druzynyRepository.GetMiejsceWDruzynie(idMiejscaWDruzynie);
            return ServiceResult<MiejsceWDruzynieDto>.Ok(new MiejsceWDruzynieDto(
                miejsce.Id,
                miejsce.DruzynaId,
                miejsce.UzytkownikId,
                miejsce.RolaId,
                miejsce.StatystykaId,
                miejsce.WartoscStatystyki,
                miejsce.WartoscLiczbowaStatystyki
            ));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<MiejsceWDruzynieDto>.NotFound(new ErrorItem(e.Message));
        }
    }

    private async Task<ServiceResult<DruzynaDoTabelkiDto>> GetDruzynaDoTabelki(int idDruzyny)
    {
        try
        {
            if(idDruzyny <= 0) return ServiceResult<DruzynaDoTabelkiDto>.BadRequest(new ErrorItem("Id drużyny musi być większe od 0"));
            var druzyna = await druzynyRepository.GetDruzyna(idDruzyny);
            
            var graRes = await wspieranaGraService.GetWspieranaGra(druzyna.GraId);
            if (!graRes.Succeeded) return ServiceResult<DruzynaDoTabelkiDto>.Fail(graRes.StatusCode, graRes.Errors);

            var ostatniaAktywnoscKapitanaRes = await uzytkownikService.GetOstatniaAktywnoscUzytkownika(druzyna.KapitanId);
            if (!ostatniaAktywnoscKapitanaRes.Succeeded) return ServiceResult<DruzynaDoTabelkiDto>.Fail(ostatniaAktywnoscKapitanaRes.StatusCode, ostatniaAktywnoscKapitanaRes.Errors);
            
            var ostatniaAktywnoscKapitana = ostatniaAktywnoscKapitanaRes.Value ?? DateTime.UtcNow;
            var minutyOdOstatniejAktywnosciKapitana = (int)(DateTime.UtcNow.AddHours(2) - ostatniaAktywnoscKapitana).TotalMinutes; // dodajemy 2 godziny, bo ostatniaAktywnoscKapitana jest w UTC, a my chcemy porównywać do czasu lokalnego (który jest UTC+2)
            // zmieniamy to na string w formacie "X dni Y godzin Z minut temu"
            var ostatniaAktywnoscKapitanaString = minutyOdOstatniejAktywnosciKapitana switch
            {
                < 2 => "teraz",
                < 5 => $"{minutyOdOstatniejAktywnosciKapitana} minuty temu",
                < 60 => $"{minutyOdOstatniejAktywnosciKapitana} minut temu",
                < 60 * 24 => $"{minutyOdOstatniejAktywnosciKapitana / 60} godzin temu",
                _ => $"{minutyOdOstatniejAktywnosciKapitana / (60 * 24)} dni temu"
            };

            var czlonkowieDruzynyRes = await GetCzlonkowieDruzynyDoWyswietlenia(idDruzyny);
            if (!czlonkowieDruzynyRes.Succeeded) return ServiceResult<DruzynaDoTabelkiDto>.Fail(czlonkowieDruzynyRes.StatusCode, czlonkowieDruzynyRes.Errors);
            var czlonkowieDruzyny = czlonkowieDruzynyRes.Value ?? new List<MiejsceWDruzynieSzczegolyDto>();

            var nastrojRozgrywki = await druzynyRepository.GetNastrojRozgrywki(druzyna.NastrojRozgrywkiId);

            return ServiceResult<DruzynaDoTabelkiDto>.Ok(new DruzynaDoTabelkiDto(
                druzyna.Id,
                druzyna.Nazwa,
                graRes.Value.Tytul, // jeżeli się powiodło, to Value nie jest null, więc można bezpiecznie użyć .Value
                ostatniaAktywnoscKapitanaString,
                minutyOdOstatniejAktywnosciKapitana,
                czlonkowieDruzyny
                    .Select(x => new MiejsceWDruzynieWTabelceDto(x.Czlonek, x.Rola, x.CzyKapitan))
                    .ToList(),
                nastrojRozgrywki.Nazwa
            ));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<DruzynaDoTabelkiDto>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<TabelkaDruzynResDto>> GetWszystkieDruzynyUzytkownikaDoTabelki(int idUzytkownika)
    {
        if(idUzytkownika <= 0) return ServiceResult<TabelkaDruzynResDto>.BadRequest(new ErrorItem("Id użytkownika musi być większe od 0"));
        var druzyny = await druzynyRepository.GetDruzynyUzytkownika(idUzytkownika);
        var idDruzyn = druzyny.Select(x => x.Id).ToArray();
        var idDruzynNaStrone = idDruzyn.Take(LiczbaDruzynNaStroneNaStart).ToArray();
        var druzynyDoTabelki = await GetDruzynyDoTabelki(idDruzynNaStrone);
        if(!druzynyDoTabelki.Succeeded) return ServiceResult<TabelkaDruzynResDto>.Fail(druzynyDoTabelki.StatusCode, druzynyDoTabelki.Errors);
                
        return ServiceResult<TabelkaDruzynResDto>.Ok(new TabelkaDruzynResDto(idDruzyn, druzynyDoTabelki.Value ?? []));
    }

    public async Task<ServiceResult<ICollection<DruzynaDoTabelkiDto>>> GetDruzynyDoTabelki(int[] idDruzyn)
    {
        var druzyny = await druzynyRepository.GetDruzyny(idDruzyn);
        var druzynyDoTabelki = new List<DruzynaDoTabelkiDto>();
        foreach (var druzyna in druzyny)
        {
            var druzynaDoTabelkiRes = await GetDruzynaDoTabelki(druzyna.Id);
            if (!druzynaDoTabelkiRes.Succeeded) return ServiceResult<ICollection<DruzynaDoTabelkiDto>>.Fail(druzynaDoTabelkiRes.StatusCode, druzynaDoTabelkiRes.Errors);
            druzynyDoTabelki.Add(druzynaDoTabelkiRes.Value); // jeżeli się powiodło, to Value nie jest null, więc można bezpiecznie użyć .Value
        }
        druzynyDoTabelki = druzynyDoTabelki
            .OrderBy(x => x.MinutyOdOstatniejAktywnosciKapitana)
            .ToList();        
        return ServiceResult<ICollection<DruzynaDoTabelkiDto>>.Ok(druzynyDoTabelki);
    }
    
    public async Task<ServiceResult<DruzynaSzczegolyDto>> PodajSzczegolyDruzyny(int idDruzyny, int idUzytkownika)
    {
        if(idDruzyny <= 0) return ServiceResult<DruzynaSzczegolyDto>.BadRequest(new ErrorItem("Id drużyny musi być większe od 0"));
        try
        {
            var druzyna = await druzynyRepository.GetDruzyna(idDruzyny);


            // pobieramy członków drużyny
            var czlonkowieDruzynyRes = await GetCzlonkowieDruzynyDoWyswietlenia(idDruzyny);
            if (!czlonkowieDruzynyRes.Succeeded)
                return ServiceResult<DruzynaSzczegolyDto>.Fail(czlonkowieDruzynyRes.StatusCode,
                    czlonkowieDruzynyRes.Errors);
            var czlonkowieDruzyny = czlonkowieDruzynyRes.Value ?? new List<MiejsceWDruzynieSzczegolyDto>();

            var statusCzlonkostwa =
                czlonkowieDruzyny.Any(x =>
                    x.Czlonek?.IdUzytkownika == idUzytkownika) // sprawdzamy, czy użytkownik jest członkiem drużyny
                    ? czlonkowieDruzyny.First(x => x.Czlonek?.IdUzytkownika == idUzytkownika)
                        .CzyKapitan // jeżeli jest członkiem, to sprawdzamy, czy jest kapitanem
                        ? "Kapitan"
                        : "Członek"
                    : "Brak"; // jeżeli nie jest członkiem

            // musimy potem jeszcze sprawdzać, czy ma zaproszenie do niej
            if (!druzyna.CzyPubliczna && statusCzlonkostwa == "Brak")
            {
                var zaproszenieRes = await powiadomienieService.CzyUzytkownikMaPowiadomienieDanegoTypuPowiazaneZObiektami(idUzytkownika, (int)TypPowiadomieniaEnum.ZaproszenieDoDruzyny,idDruzyny, null);
                if (!zaproszenieRes.Succeeded) return ServiceResult<DruzynaSzczegolyDto>.Fail(zaproszenieRes.StatusCode, zaproszenieRes.Errors);
                // jeżeli nie ma zaproszenia do tej drużyny, to nie może zobaczyć jej szczegółów, bo jest prywatna
                if(!zaproszenieRes.Value) return ServiceResult<DruzynaSzczegolyDto>.Forbidden(new ErrorItem(
                    "Nie można pobrać szczegółów drużyny, ponieważ jest ona prywatna, a użytkownik nie jest jej członkiem"));
            }

            // pobieramy grę, żeby mieć jej tytuł
            var graRes = await wspieranaGraService.GetWspieranaGra(druzyna.GraId);
            if (!graRes.Succeeded) return ServiceResult<DruzynaSzczegolyDto>.Fail(graRes.StatusCode, graRes.Errors);

            // pobieramy nastrój rozgrywki
            var nastrojRozgrywki = await druzynyRepository.GetNastrojRozgrywki(druzyna.NastrojRozgrywkiId);

            // pobieramy wymageny język i stopień biegłości, żeby mieć ich nazwy
            var jezykRes = druzyna.WymaganyJezykId != null
                ? await jezykService.GetJezyk(druzyna.WymaganyJezykId ??
                                              0) // już odfiltrowaliśmy drużyny bez WymaganyJezykId, więc możemy bezpiecznie użyć ?? 0
                : null;

            if (jezykRes is { Succeeded: false })
                return ServiceResult<DruzynaSzczegolyDto>.Fail(jezykRes.StatusCode, jezykRes.Errors);

            var stopienBieglosciRes = druzyna.WymaganyStopienBieglosciJezykaId != null
                ? await stopienBieglosciJezykaService.GetStopienBieglosciJezyka(
                    druzyna.WymaganyStopienBieglosciJezykaId ??
                    0) // już odfiltrowaliśmy drużyny bez WymaganyStopienBieglosciJezykaId, więc możemy bezpiecznie użyć ?? 0
                : null;

            if (stopienBieglosciRes is { Succeeded: false })
                return ServiceResult<DruzynaSzczegolyDto>.Fail(stopienBieglosciRes.StatusCode,
                    stopienBieglosciRes.Errors);


            var jezykIStopienBiegłosci = jezykRes != null
                ? stopienBieglosciRes != null
                    ? $"{jezykRes.Value.Nazwa} - {stopienBieglosciRes.Value.Nazwa}" // już odfiltrowaliśmy drużyny bez WymaganyJezykId i WymaganyStopienBieglosciJezykaId, więc możemy bezpiecznie użyć .Value
                    : jezykRes.Value
                        .Nazwa // już odfiltrowaliśmy drużyny bez WymaganyJezykId, więc możemy bezpiecznie użyć .Value
                : null; // jeżeli nie ma wymaganego języka, to zwracamy null

            // pobieramy wymagania drużyny
            var wymaganiaRes = await statystykiService.GetWymaganiaDruzynyDoWyswietlenia(idDruzyny);
            if (!wymaganiaRes.Succeeded)
                return ServiceResult<DruzynaSzczegolyDto>.Fail(wymaganiaRes.StatusCode, wymaganiaRes.Errors);

            // pobieramy platformę, żeby mieć jej nazwę i logo
            string? nazwaPlatformy = null;
            byte[]? logoPlatformy = null;
            if (druzyna.PlatformaId != null)
            {
                var platformaRes =
                    await platformaService.GetPlatforma(druzyna.PlatformaId ??
                                                        0); // już odfiltrowaliśmy drużyny bez PlatformaId, więc możemy bezpiecznie użyć ?? 0
                if (!platformaRes.Succeeded)
                    return ServiceResult<DruzynaSzczegolyDto>.Fail(platformaRes.StatusCode, platformaRes.Errors);
                nazwaPlatformy =
                    platformaRes.Value
                        .Nazwa; // już odfiltrowaliśmy drużyny bez PlatformaId, więc możemy bezpiecznie użyć .Value
                logoPlatformy = platformaRes.Value.Logo;
            }

            var czyUzytkownikSpelniaWymaganiaCzlonkostwaRes =
                await CzyUzytkownikSpelniaWymaganiaDruzyny(idDruzyny, idUzytkownika);
            if (!czyUzytkownikSpelniaWymaganiaCzlonkostwaRes.Succeeded)
                return ServiceResult<DruzynaSzczegolyDto>.Fail(wymaganiaRes.StatusCode, wymaganiaRes.Errors);
            var czyUzytkownikSpelniaWymaganiaCzlonkostwa = czyUzytkownikSpelniaWymaganiaCzlonkostwaRes.Value;

            var czlonkowieDruzynyZeSprawdzonymiWymaganiami = new List<MiejsceWDruzynieSzczegolyDto>();
            if (statusCzlonkostwa != "Brak")
                czlonkowieDruzynyZeSprawdzonymiWymaganiami
                    .AddRange(czlonkowieDruzyny); // jak to nasza drużyna, nie musimy sprawdzać
            else
            {
                // najpierw dodajemy zapełnione miejsca, bo dla nich nie musimy sprawdzać wymagań, więc od razu możemy je dodać do listy ze sprawdzonymi wymaganiami
                czlonkowieDruzynyZeSprawdzonymiWymaganiami.AddRange(czlonkowieDruzyny.Where(x => x.Czlonek != null));

                foreach (var miejsceWDruzynie in czlonkowieDruzyny.Where(x => x.Czlonek == null))
                {
                    if (!czyUzytkownikSpelniaWymaganiaCzlonkostwa)
                    {
                        // jeżeli nie spełnia wymagań członkostwa, to nie spełnia też wymagań miejsca
                        czlonkowieDruzynyZeSprawdzonymiWymaganiami.Add(miejsceWDruzynie with
                        {
                            CzyOgladajacySpelniaWymagania = false
                        });
                        continue;
                    }

                    var czySpelniaWymaganiaRes =
                        await CzyUzytkownikSpelniaWymaganieMiejsca(miejsceWDruzynie.IdMiejscaWDruzynie, idUzytkownika);
                    if (!czySpelniaWymaganiaRes.Succeeded)
                        czlonkowieDruzynyZeSprawdzonymiWymaganiami.Add(new MiejsceWDruzynieSzczegolyDto(
                            miejsceWDruzynie.IdMiejscaWDruzynie,
                            miejsceWDruzynie.Czlonek,
                            miejsceWDruzynie.Rola,
                            miejsceWDruzynie.Wymaganie,
                            miejsceWDruzynie.CzyKapitan,
                            null // jeżeli nie udało się sprawdzić, to zostawiamy null, żeby frontend mógł zdecydować, co z tym zrobić
                        ));

                    else
                        czlonkowieDruzynyZeSprawdzonymiWymaganiami.Add(miejsceWDruzynie with
                        {
                            CzyOgladajacySpelniaWymagania = czySpelniaWymaganiaRes.Value
                        });
                }
            }


            // składamy wszystko do kupy i zwracamy szczegóły drużyny
            return ServiceResult<DruzynaSzczegolyDto>.Ok(new DruzynaSzczegolyDto(
                druzyna.Nazwa,
                graRes.Value.Tytul, // jeżeli się powiodło, to Value nie jest null, więc można bezpiecznie użyć .Value
                druzyna.Opis,
                nastrojRozgrywki.Nazwa,
                nastrojRozgrywki.Id,
                czlonkowieDruzynyZeSprawdzonymiWymaganiami,
                jezykIStopienBiegłosci,
                wymaganiaRes.Value, // już się upewniliśmy, że wymaganiaRes.Value nie jest null, więc można bezpiecznie użyć .Value
                druzyna.CzyPubliczna,
                nazwaPlatformy,
                logoPlatformy,
                statusCzlonkostwa,
                druzyna.CzyZintegrowano
            ));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<DruzynaSzczegolyDto>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<ICollection<MiejsceWDruzynieSzczegolyDto>>> GetCzlonkowieDruzynyDoWyswietlenia(int idDruzyny)
    {
        if(idDruzyny <= 0) return ServiceResult<ICollection<MiejsceWDruzynieSzczegolyDto>>.BadRequest(new ErrorItem("Id drużyny musi być większe od 0"));
        var druzyna = await druzynyRepository.GetDruzyna(idDruzyny);
        
        var miejscaWDruzynie = await druzynyRepository.GetMiejscaWDruzynie(idDruzyny);
        
        var czlonkowieDoZwrocenia = new List<MiejsceWDruzynieSzczegolyDto>();
        foreach (var miejsce in miejscaWDruzynie)
        {
            ProfilMinInfoDto? czlonek = null;
            if (miejsce.UzytkownikId != null)
            { 
                var profilRes = await profilService.GetProfilMinInfo(miejsce.UzytkownikId ?? 0); // już odfiltrowaliśmy miejsca bez UzytkownikId
                if(!profilRes.Succeeded) return ServiceResult<ICollection<MiejsceWDruzynieSzczegolyDto>>.Fail(profilRes.StatusCode, profilRes.Errors);
                czlonek = profilRes.Value ?? null; // jeżeli się powiodło, to Value nie jest null, więc można bezpiecznie użyć .Value
            }
            
            string? wymaganie = null;
            if (miejsce.StatystykaId != null)
            {
                var statystykaRes = await statystykiService.GetStatystyka(miejsce.StatystykaId ?? 1); // już odfiltrowaliśmy miejsca bez StatystykaId, więc możemy bezpiecznie użyć ?? 1,
                if(!statystykaRes.Succeeded) return ServiceResult<ICollection<MiejsceWDruzynieSzczegolyDto>>.Fail(statystykaRes.StatusCode, statystykaRes.Errors);
                wymaganie = miejsce.RolaId != null 
                        ? $"{statystykaRes.Value.Nazwa}({miejsce.Rola?.Nazwa}): {miejsce.WartoscStatystyki}" // juz odfiltrowaliśmy miejsca bez StatystykaId, więc możemy bezpiecznie użyć .Value
                        : $"{statystykaRes.Value.Nazwa}: {miejsce.WartoscStatystyki}";
            }
            
            czlonkowieDoZwrocenia.Add(new MiejsceWDruzynieSzczegolyDto(
                miejsce.Id,
                czlonek,
                miejsce.Rola?.Nazwa,
                wymaganie,
                druzyna.KapitanId == miejsce.UzytkownikId
            ));
        }

        return ServiceResult<ICollection<MiejsceWDruzynieSzczegolyDto>>.Ok(czlonkowieDoZwrocenia);
    }
    
    // funkcja przekazująca do frontu dane potrzebne do formularza drużyny
    // potrzebujemy: listę nastrojów rozgrywki, listę platform, listę języków wraz ze stopniami biegłości języka, listę ról, listę statystyk
    public async Task<ServiceResult<DaneDoFormularzaDruzynyZeStatystykamiDto>> GetDaneDoFormularzaDruzynyZeStatystykami(int idGry, int idUzytkownika)
    {
        // sprawdzamy, czy podane dane są okej
        var graRes = await wspieranaGraService.GetWspieranaGra(idGry);
        if (!graRes.Succeeded) return ServiceResult<DaneDoFormularzaDruzynyZeStatystykamiDto>.Fail(graRes.StatusCode, graRes.Errors);
        
        var uzytkownikRes = await uzytkownikService.GetUzytkownik(idUzytkownika);
        if (!uzytkownikRes.Succeeded) return ServiceResult<DaneDoFormularzaDruzynyZeStatystykamiDto>.Fail(uzytkownikRes.StatusCode, uzytkownikRes.Errors);
        
        var nastroje = await druzynyRepository.GetNastrojeRozgrywki();
        
        var platformyRes = await wspieranaGraService.GetPlatformyGryUzytkownika(idGry, idUzytkownika);
        if (!platformyRes.Succeeded) return ServiceResult<DaneDoFormularzaDruzynyZeStatystykamiDto>.Fail(platformyRes.StatusCode, platformyRes.Errors);
        
        var jezykiOrazStopnieRes = await jezykService.GetJezykiProfiluZRownymiLubNizszymiStopniami(idUzytkownika);
        if (!jezykiOrazStopnieRes.Succeeded) return ServiceResult<DaneDoFormularzaDruzynyZeStatystykamiDto>.Fail(jezykiOrazStopnieRes.StatusCode, jezykiOrazStopnieRes.Errors);
        
        var roleRes = await statystykiService.GetRoleGry(idGry);
        if (!roleRes.Succeeded) return ServiceResult<DaneDoFormularzaDruzynyZeStatystykamiDto>.Fail(roleRes.StatusCode, roleRes.Errors);
        
        var statystykiRes = await statystykiService.GetStatystykiDoFormularza(idGry, idUzytkownika);
        if (!statystykiRes.Succeeded) return ServiceResult<DaneDoFormularzaDruzynyZeStatystykamiDto>.Fail(statystykiRes.StatusCode, statystykiRes.Errors);
        
        return ServiceResult<DaneDoFormularzaDruzynyZeStatystykamiDto>.Ok(new DaneDoFormularzaDruzynyZeStatystykamiDto(
            nastroje.Select(x => new NastrojRozgrywkiDto(x.Id, x.Nazwa)).ToList(),
            platformyRes.Value, // jeżeli się powiodło, to Value nie jest null, więc można bezpiecznie użyć .Value
            jezykiOrazStopnieRes.Value, // jeżeli się powiodło, to Value nie jest null, więc można bezpiecznie użyć .Value
            roleRes.Value, // podobnie co wyżej i niżej
            statystykiRes.Value // jeżeli się powiodło, to Value nie jest null, więc można bezpiecznie użyć .Value
        ));
    }

    // funkcja zwracająca dane do formularza bez statystyk, nie spersonalizowane
    public async Task<ServiceResult<DaneDoFormularzaDruzynyBezStatystykDto>> GetDaneDoFormularzaDruzynyBezStatystyk(int idGry)
    {
        // sprawdzamy, czy podane id jest okej
        var graRes = await wspieranaGraService.GetWspieranaGra(idGry);
        if (!graRes.Succeeded) return ServiceResult<DaneDoFormularzaDruzynyBezStatystykDto>.Fail(graRes.StatusCode, graRes.Errors);
        
        var nastroje = await druzynyRepository.GetNastrojeRozgrywki();
        var platformyRes = await wspieranaGraService.GetPlatformyGry(idGry);
        if (!platformyRes.Succeeded) return ServiceResult<DaneDoFormularzaDruzynyBezStatystykDto>.Fail(platformyRes.StatusCode, platformyRes.Errors);
        
        var jezykiRes = await jezykService.GetJezyki();
        if (!jezykiRes.Succeeded) return ServiceResult<DaneDoFormularzaDruzynyBezStatystykDto>.Fail(jezykiRes.StatusCode, jezykiRes.Errors);
        
        var stopnieRes = await stopienBieglosciJezykaService.GetStopnieBieglosciJezyka();
        if (!stopnieRes.Succeeded) return ServiceResult<DaneDoFormularzaDruzynyBezStatystykDto>.Fail(stopnieRes.StatusCode, stopnieRes.Errors);
        
        var roleRes = await statystykiService.GetRoleGry(idGry);
        if (!roleRes.Succeeded) return ServiceResult<DaneDoFormularzaDruzynyBezStatystykDto>.Fail(roleRes.StatusCode, roleRes.Errors);

        return ServiceResult<DaneDoFormularzaDruzynyBezStatystykDto>.Ok(
            new DaneDoFormularzaDruzynyBezStatystykDto(
                nastroje.Select(x => new NastrojRozgrywkiDto(x.Id, x.Nazwa)).ToList(),
                platformyRes.Value, // jeżeli się powiodło, to Value nie jest null, więc można bezpiecznie użyć .Value
                jezykiRes.Value, // jeżeli się powiodło, to Value nie jest null, więc można bezpiecznie użyć .Value
                stopnieRes.Value, // jeżeli się powiodło, to Value nie jest null, więc można bezpiecznie użyć .Value
                roleRes.Value // jeżeli się powiodło, to Value nie jest null, więc można bezpiecznie użyć .Value
            )
        );
    }
    
    // funkcja zwracająca dane do formularza bez statystyk, nie spersonalizowane
    public async Task<ServiceResult<DaneDoFormularzaWyszukiwaniaDruzyny>> GetDaneDoFormularzaWyszukiwaniaDruzyny(int idUzytkownika)
    {

        var gryRes = await wspieranaGraService.GetWspieraneGryZPlatformamiDoSelect();
        
        var bibliotekaGierRes = await bibliotekaGierService.PodajGryWBiblioteceUzytkownika(idUzytkownika);
        if (!bibliotekaGierRes.Succeeded) return ServiceResult<DaneDoFormularzaWyszukiwaniaDruzyny>.Fail(bibliotekaGierRes.StatusCode, bibliotekaGierRes.Errors);
        
        var gryUzytkownikaZPlatformami = bibliotekaGierRes.Value.Select(x =>
            new GraZPlatformaDoSelectDto(
                x.IdGry, 
                x.Tytul, 
                x.Platformy.Select(y => new PlatformaMinInfo(y.IdPlatformy, y.Nazwa)).ToList()
            )
        ).ToList(); // jeżeli się powiodło, to Value nie jest null, więc można bezpiecznie użyć .Value
        
        var nastroje = await druzynyRepository.GetNastrojeRozgrywki();
        
        var platformyRes = await wspieranaGraService.GetWspieraneGryZPlatformamiDoSelect();
        if (!platformyRes.Succeeded) return ServiceResult<DaneDoFormularzaWyszukiwaniaDruzyny>.Fail(platformyRes.StatusCode, platformyRes.Errors);
        
        var jezykiOrazStopnieRes = await jezykService.GetJezykiProfiluZRownymiLubNizszymiStopniami(idUzytkownika);
        if (!jezykiOrazStopnieRes.Succeeded) return ServiceResult<DaneDoFormularzaWyszukiwaniaDruzyny>.Fail(jezykiOrazStopnieRes.StatusCode, jezykiOrazStopnieRes.Errors);

        var roleRes = await statystykiService.GetRole();
        if (!roleRes.Succeeded) return ServiceResult<DaneDoFormularzaWyszukiwaniaDruzyny>.Fail(roleRes.StatusCode, roleRes.Errors);

        return ServiceResult<DaneDoFormularzaWyszukiwaniaDruzyny>.Ok(
            new DaneDoFormularzaWyszukiwaniaDruzyny(
                gryRes.Value, // jeżeli się powiodło, to Value nie jest null, więc można bezpiecznie użyć .Value
                gryUzytkownikaZPlatformami,
                nastroje.Select(x => new NastrojRozgrywkiDto(x.Id, x.Nazwa)).ToList(),
                jezykiOrazStopnieRes.Value, // jeżeli się powiodło, to Value nie jest null, więc można bezpiecznie użyć .Value
                roleRes.Value // jeżeli się powiodło, to Value nie jest null, więc można bezpiecznie użyć .Value
            )
        );
    }
    public async Task<ServiceResult<ICollection<NastrojRozgrywkiDto>>> GetNastrojeRozgrywki()
    {
        var nastroje = await druzynyRepository.GetNastrojeRozgrywki();
        var nastrojeDto = nastroje.Select(x => new NastrojRozgrywkiDto(x.Id, x.Nazwa)).ToList();
        return ServiceResult<ICollection<NastrojRozgrywkiDto>>.Ok(nastrojeDto);
    }
    public async Task<ServiceResult<bool>> CzyUzytkownikSpelniaWymaganieMiejsca(int idMiejsca, int idUzytkownika)
    {
        if(idMiejsca <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id miejsca w drużynie: " + idMiejsca));
        if(idUzytkownika <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id użytkownika: " + idUzytkownika)); 
        try 
        { 
            var spelniaWymaganie = await druzynyRepository.CzyUzytkownikSpelniaWymaganieMiejsca(idMiejsca, idUzytkownika); 
            return ServiceResult<bool>.Ok(spelniaWymaganie);
        }
        catch (NieZnalezionoWBazieException e)
        {
         return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> CzyUzytkownikSpelniaWymaganiaDruzyny(int idDruzyny, int idUzytkownika)
    {
        if(idDruzyny <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id drużyny: " + idDruzyny));
        if(idUzytkownika <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id użytkownika: " + idUzytkownika)); 
        try 
        { 
            var druzyna = await druzynyRepository.GetDruzyna(idDruzyny);
            if (!druzyna.CzyZintegrowano) return ServiceResult<bool>.Ok(true); // jeżeli drużyna nie jest zintegrowana, to każdy spełnia jej wymagania
            
            // sprawdzamy, czy ma daną grę na danej platformie
            if(druzyna.PlatformaId != null){
                var czyMaTeGreNaPlatformieRes = await bibliotekaGierService.CzyUzytkownikMaDanaGreNaDanejPlatformie(
                    idUzytkownika, 
                    druzyna.GraId,
                    druzyna.PlatformaId ?? 0 // już odfiltrowaliśmy drużyny bez PlatformaId, więc możemy bezpiecznie użyć ?? 0
                ); 
                if (!czyMaTeGreNaPlatformieRes.Succeeded) return czyMaTeGreNaPlatformieRes;
                if (!czyMaTeGreNaPlatformieRes.Value) return ServiceResult<bool>.Ok(false);
            }
            
            var spelniaWymaganie = await druzynyRepository.CzyUzytkownikSpelniaWymaganiaDruzyny(idDruzyny, idUzytkownika); 
            return ServiceResult<bool>.Ok(spelniaWymaganie);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }

    
    public async Task<ServiceResult<bool>> CzyUzytkownikPrzekraczaMaksLiczbeDruzyn(int idUzytkownika, int idGry)
    {
        if(idGry <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id gry: " + idGry));
        if(idUzytkownika <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id użytkownika: " + idUzytkownika));
        
        try{
            
            return ServiceResult<bool>.Ok(await druzynyRepository.CzyUzytkownikPrzekraczaMaksLiczbeDruzyn(idGry, idUzytkownika));
            
        }catch(NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> CzyUzytkownikNalezyDoDruzyny(int idUzytkownika, int idDruzyny)
    {
        if(idDruzyny <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id drużyny: " + idDruzyny));
        if(idUzytkownika <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id użytkownika: " + idUzytkownika));
        
        try{
            
            return ServiceResult<bool>.Ok(await druzynyRepository.CzyUzytkownikNalezyDoDruzyny(idDruzyny, idUzytkownika));
            
        }catch(NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> StworzDruzyne(CreateDruzynaReqDto druzynaReq, int idKapitana)
    {
        try
        {
            // sprawdzamy poprawność danych
            
            // tutaj tylko na błędy nazwy i opisu
            var bledy = new List<ErrorItem>();
            if (string.IsNullOrWhiteSpace(druzynaReq.Nazwa)) bledy.Add(new ErrorItem("Nazwa drużyny nie może być pusta", nameof(druzynaReq.Nazwa)));
            if (druzynaReq.Nazwa.Length > 40) bledy.Add(new ErrorItem("Nazwa drużyny nie może być dłuższa niż 40 znaków", nameof(druzynaReq.Nazwa)));
            if (druzynaReq.Opis?.Length > 300) bledy.Add(new ErrorItem("Opis drużyny nie może być dłuższy niż 300 znaków", nameof(druzynaReq.Opis)));
            if(bledy.Count > 0) return ServiceResult<bool>.BadRequest(bledy.ToArray());
            
            var graRes = await wspieranaGraService.GetWspieranaGra(druzynaReq.IdGry);
            if (!graRes.Succeeded) return ServiceResult<bool>.Fail(graRes.StatusCode, graRes.Errors);

            var czyUzytkownikPrzekraczaMaksLiczbeDruzynRes = await CzyUzytkownikPrzekraczaMaksLiczbeDruzyn(druzynaReq.IdGry, idKapitana);
            if (!czyUzytkownikPrzekraczaMaksLiczbeDruzynRes.Succeeded) return ServiceResult<bool>.Fail(czyUzytkownikPrzekraczaMaksLiczbeDruzynRes.StatusCode, czyUzytkownikPrzekraczaMaksLiczbeDruzynRes.Errors);
            if (czyUzytkownikPrzekraczaMaksLiczbeDruzynRes.Value) return ServiceResult<bool>.BadRequest(new ErrorItem("Nie można stworzyć drużyny, ponieważ użytkownik może być w maksymalnie " + DruzynyRepository.MaksymalnaLiczbaDruzynGraczaDlaGry +" drużynach dla danej gry"));
            
            if (druzynaReq.IdPlatformy != null)
            {
                var platformaRes =
                    await platformaService.GetPlatforma(druzynaReq.IdPlatformy ??
                                                        0); // już odfiltrowaliśmy drużyny bez IdPlatformy, więc możemy bezpiecznie użyć ?? 0
                if (!platformaRes.Succeeded)
                    return ServiceResult<bool>.Fail(platformaRes.StatusCode, platformaRes.Errors);
            }

            if (druzynaReq.IdWymaganegoJezyka != null)
            {
                var jezykRes = await jezykService.GetJezyk(druzynaReq.IdWymaganegoJezyka ?? 0);
                if (!jezykRes.Succeeded) return ServiceResult<bool>.Fail(jezykRes.StatusCode, jezykRes.Errors);
            }

            if (druzynaReq.IdWymaganegoStopniaBieglosciJezyka != null)
            {
                var stopienRes =
                    await stopienBieglosciJezykaService.GetStopienBieglosciJezyka(
                        druzynaReq.IdWymaganegoStopniaBieglosciJezyka ?? 0);
                if (!stopienRes.Succeeded) return ServiceResult<bool>.Fail(stopienRes.StatusCode, stopienRes.Errors);
            }

            if (druzynaReq.IdRoliKapitana != null)
            {
                var rolaRes = await statystykiService.GetRola(druzynaReq.IdRoliKapitana ?? 0);
                if (!rolaRes.Succeeded) return ServiceResult<bool>.Fail(rolaRes.StatusCode, rolaRes.Errors);
            }

            if (druzynaReq.WymaganeStatystyki != null)
            {
                var nieprawidloweStatystykiRes =
                    statystykiService.FiltrujNieistniejaceStatystyki(druzynaReq.WymaganeStatystyki
                        .Select(x => x.IdStatystyki).ToList());
                if (!nieprawidloweStatystykiRes.Succeeded)
                    return ServiceResult<bool>.Fail(nieprawidloweStatystykiRes.StatusCode,
                        nieprawidloweStatystykiRes.Errors);
                var nieprawidloweStatystyki = nieprawidloweStatystykiRes.Value ?? [];
                if (nieprawidloweStatystyki.Count > 0)
                {
                    var errorMessage =
                        $"Podane statystyki o id: [{string.Join(", ", nieprawidloweStatystyki)}], które zostały podane w wymaganiach drużyny, nie istnieją w bazie danych.";
                    return ServiceResult<bool>.BadRequest(new ErrorItem(errorMessage));
                }

                var czySpelniaWymaganiaRes = await statystykiService.CzyUzytkownikSpelniaWymagania(druzynaReq.WymaganeStatystyki, idKapitana);
                if(!czySpelniaWymaganiaRes.Succeeded) return ServiceResult<bool>.Fail(czySpelniaWymaganiaRes.StatusCode, czySpelniaWymaganiaRes.Errors);
                if (!czySpelniaWymaganiaRes.Value) return ServiceResult<bool>.BadRequest(new ErrorItem("Kapitan musi spełniać wymagania tworzonej przez siebie drużyny"));
            }

            var wymaganeStatystykiMiejscWDruzynie = druzynaReq.MiejscaWDruzynie.Select(x => x.WymaganaStatystyka)
                .Where(x => x != null).Select(x => x!.IdStatystyki).ToList();
            if (wymaganeStatystykiMiejscWDruzynie.Count > 0)
            {
                var nieprawidloweStatystykiRes =
                    statystykiService.FiltrujNieistniejaceStatystyki(wymaganeStatystykiMiejscWDruzynie);
                if (!nieprawidloweStatystykiRes.Succeeded)
                    return ServiceResult<bool>.Fail(nieprawidloweStatystykiRes.StatusCode,
                        nieprawidloweStatystykiRes.Errors);
                var nieprawidloweStatystyki = nieprawidloweStatystykiRes.Value ?? [];
                if (nieprawidloweStatystyki.Count > 0)
                {
                    var errorMessage =
                        $"Podane statystyki w miejscach w drużynie o id statystyk: [{string.Join(", ", nieprawidloweStatystyki)}] nie istnieją w bazie danych.";
                    return ServiceResult<bool>.BadRequest(new ErrorItem(errorMessage));
                }
            }

            // statystyki sprawdzone, tworzymy drużynę
            var stworzDruzyneRes = await druzynyRepository.StworzDruzyne(druzynaReq, idKapitana);
            if (!stworzDruzyneRes) return ServiceResult<bool>.Fail(500, [new ErrorItem("Nie udało się stworzyć drużyny")]);
            
            return ServiceResult<bool>.Ok(true);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> UsunDruzyne(int idDruzyny, int idUsuwajacegoUzytkownika)
    {
        try
        {
            if (idDruzyny <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id drużyny: " + idDruzyny)); 
            
            var druzyna = await druzynyRepository.GetDruzyna(idDruzyny);
            if (druzyna.KapitanId != idUsuwajacegoUzytkownika) return ServiceResult<bool>.Forbidden(new ErrorItem("Tylko kapitan drużyny może ją usunąć"));
            
            var usunDruzyneRes = await druzynyRepository.UsunDruzyne(idDruzyny);
            if (!usunDruzyneRes) return ServiceResult<bool>.Fail(500, [new ErrorItem("Nie udało się usunąć drużyny")]);
            
            return ServiceResult<bool>.Ok(true);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> OpuscDruzyne(int idDruzyny, int idUzytkownika)
    {
        try
        {
            if (idDruzyny <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id drużyny: " + idDruzyny)); 
            if (idUzytkownika <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id użytkownika: " + idUzytkownika)); 
            
            var druzyna = await druzynyRepository.GetDruzyna(idDruzyny);
            if (druzyna.KapitanId == idUzytkownika) return ServiceResult<bool>.Forbidden(new ErrorItem("Kapitan drużyny nie może jej opuścić, musi ją usunąć"));
            
            var rola = await druzynyRepository.GetRolaMiejscaWDruzynieUzytkownika(idDruzyny, idUzytkownika);

            var opuscDruzyneRes = await druzynyRepository.OpuscDruzyne(idDruzyny, idUzytkownika);
            if (!opuscDruzyneRes) return ServiceResult<bool>.Fail(500, [new ErrorItem("Nie udało się opuścić drużyny")]);

            // wysyłamy kapitanowi powiadomienie, że ktoś opuścił drużynę
            var powiadomienieRes = await powiadomienieService.WyslijPowiadomienieOWyjsciuZDruzyny(
                druzyna.KapitanId,
                idUzytkownika,
                druzyna.Id,
                druzyna.Nazwa,
                rola?.Nazwa
            );
            if (!powiadomienieRes.Succeeded) return ServiceResult<bool>.Fail(powiadomienieRes.StatusCode, powiadomienieRes.Errors);
            
            return ServiceResult<bool>.NoContent(true);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> OproznijMiejsceWDruzynie(int idMiejsca, int idUsuwajacegoUzytkownika)
    {
        try
        {
            if (idMiejsca <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id miejsca: " + idMiejsca)); 
            
            var kapitanId = await druzynyRepository.GetIdKapitanaDruzynyMiejsca(idMiejsca);
            if (kapitanId != idUsuwajacegoUzytkownika) return ServiceResult<bool>.Forbidden(new ErrorItem("Tylko kapitan drużyny może wyrzucić użytkownika z drużyny"));
            
            var miejsce = await druzynyRepository.GetMiejsceWDruzynie(idMiejsca);
            if(miejsce.UzytkownikId == idUsuwajacegoUzytkownika) return ServiceResult<bool>.Forbidden(new ErrorItem("Kapitan drużyny nie może jej opuścić, musi ją usunąć"));
            
            var usuwanyUzytkownikId = miejsce.UzytkownikId;
            
            var opuscDruzyneRes = await druzynyRepository.OproznijMiejsceWDruzynie(idMiejsca);
            if (!opuscDruzyneRes) return ServiceResult<bool>.Fail(500, [new ErrorItem("Nie udało się usunąć użytkownika z miejsca o id " + idMiejsca)]);
            
            // wysyłamy wyrzuconemu powiadomienie, że został wyrzucony z drużyny
            var druzynaRes = await GetDruzyna(miejsce.DruzynaId);
            if(!druzynaRes.Succeeded) return ServiceResult<bool>.Fail(druzynaRes.StatusCode, druzynaRes.Errors);

            var powiadomienieRes = await powiadomienieService.WyslijPowiadomienieOUsunieciuZDruzyny(
                usuwanyUzytkownikId ?? 0, // już odfiltrowaliśmy miejsca bez UzytkownikId, więc możemy bezpiecznie użyć ?? 0
                miejsce.DruzynaId,
                druzynaRes.Value.Nazwa
            );
            if (!powiadomienieRes.Succeeded) return ServiceResult<bool>.Fail(powiadomienieRes.StatusCode, powiadomienieRes.Errors);
            
            return ServiceResult<bool>.Ok(true);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> WyrzucUzytkownikaZeWszystkichDruzyn(int idUzytkownika)
    {
        if (idUzytkownika <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id użytkownika: " + idUzytkownika)); 
        
        var wyrzucUzytkownikaRes = await druzynyRepository.WyrzucUzytkownikaZeWszystkichDruzyn(idUzytkownika);
        if (!wyrzucUzytkownikaRes) return ServiceResult<bool>.Fail(500, [new ErrorItem("Nie udało się wyrzucić użytkownika ze wszystkich drużyn")]);
        
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<bool>> PrzerwijIntegracjeUzytkownikaOdnosnieDruzyn(int idUzytkownika)
    {
        if (idUzytkownika <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id użytkownika: " + idUzytkownika)); 
        
        var wyrzucUzytkownikaRes = await druzynyRepository.WyrzucUzytkownikaZeWszystkichZintegrowanychDruzyn(idUzytkownika);
        if (!wyrzucUzytkownikaRes) return ServiceResult<bool>.Fail(500, [new ErrorItem("Nie udało się wyrzucić użytkownika ze wszystkich zintegrowanych drużyn")]);
        var usunDruzynyRes = await druzynyRepository.UsunWszystkieZintegrowaneDruzynyUzytkownika(idUzytkownika);
        if (!usunDruzynyRes) return ServiceResult<bool>.Fail(500, [new ErrorItem("Nie udało się usunąć wszystkich zintegrowanych drużyn użytkownika")]);
        
        return ServiceResult<bool>.Ok(true);
    }
    
    public async Task<ServiceResult<bool>> UsunWszystkieDruzynyUzytkownika(int idUzytkownika)
    {
        if (idUzytkownika <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id użytkownika: " + idUzytkownika)); 
        
        var result = await druzynyRepository.UsunWszystkieDruzynyUzytkownika(idUzytkownika);
        if (!result) return ServiceResult<bool>.Fail(500, [new ErrorItem("Nie udało się usunąć wszystkich drużyn użytkownika")]);
        return  ServiceResult<bool>.NoContent(true);
    }

    public async Task<ServiceResult<bool>> UpdateDruzyna(int idDruzyny, int idUzytkownika, DruzynaUpdateDto druzynaReq)
    {
        if(idDruzyny <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Id drużyny musi być większe od 0"));
        try
        {
            var druzyna = await druzynyRepository.GetDruzyna(idDruzyny);
            if(druzyna.KapitanId != idUzytkownika) return ServiceResult<bool>.Forbidden(new ErrorItem("Tylko kapitan drużyny może ją edytować"));
            
            var nastroj = await druzynyRepository.GetNastrojRozgrywki(druzynaReq.IdNastrojuRozgrywki); // tylko po to, aby wywaliło błąd gdy nie znajdzie
            
            var bledy = new List<ErrorItem>();

            if(druzynaReq.Nazwa.Trim().Length == 0) bledy.Add(new ErrorItem("Nazwa drużyny nie może być pusta", nameof(druzynaReq.Nazwa)));
            if(druzynaReq.Nazwa.Trim().Length > 40) bledy.Add(new ErrorItem("Nazwa drużyny nie może być dłuższa niż 40 znaków", nameof(druzynaReq.Nazwa)));
            if(druzynaReq.Opis?.Trim().Length > 300) bledy.Add(new ErrorItem("Opis nie może być dłuższy niż 300 znaków", nameof(druzynaReq.Opis)));
            
            return bledy.Count > 0 
                ? ServiceResult<bool>.BadRequest(bledy.ToArray()) 
                : ServiceResult<bool>.NoContent(await druzynyRepository.UpdateDruzyna(idDruzyny, druzynaReq));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<TabelkaDruzynResDto>> WyszukajDruzyny(WyszukajDruzyneReqDto req, int idUzytkownika)
    {
        // sprawdzić, czy jeżeli jest zintegrowane, to czy ma tę grę i platformę. może wyżej też?
        // odfiltrowujemy tak, aby zostało bez ról tylko wtedy gdy faktycznie nie ma ról
        
        // preferencje zintegrowania = [zintegrowane, niezintegrowane, wszystkie]

        var czyUzytkownikMaZintegrowaneKontoRes = await uzytkownikService.CzyUzytkownikMaZintegrowaneKonto(idUzytkownika);
        if (!czyUzytkownikMaZintegrowaneKontoRes.Succeeded) return ServiceResult<TabelkaDruzynResDto>.Fail(czyUzytkownikMaZintegrowaneKontoRes.StatusCode, czyUzytkownikMaZintegrowaneKontoRes.Errors);
        var maZintegrowaneKonto = czyUzytkownikMaZintegrowaneKontoRes.Value;
        if(!maZintegrowaneKonto && req.PreferencjeZintegrowania != "niezintegrowane")
            return ServiceResult<TabelkaDruzynResDto>.BadRequest(new ErrorItem("Nie można szukać drużyn zintegrowanych, ponieważ nie masz zintegrowanego konta"));
        
        var gra = await wspieranaGraService.GetWspieranaGra(req.IdGry);
        if (!gra.Succeeded) return ServiceResult<TabelkaDruzynResDto>.Fail(gra.StatusCode, gra.Errors);
        
        if(req.IdPlatformy != null)
        {
            var platforma = await platformaService.GetPlatforma(req.IdPlatformy ?? 0);
            if (!platforma.Succeeded) return ServiceResult<TabelkaDruzynResDto>.Fail(platforma.StatusCode, platforma.Errors);
            var czyMaTeGreNaPlatformieRes = await bibliotekaGierService.CzyUzytkownikMaDanaGreNaDanejPlatformie(
                idUzytkownika, 
                req.IdGry,
                req.IdPlatformy ?? 0 // już odfiltrowaliśmy drużyny bez IdPlatformy, więc możemy bezpiecznie użyć ?? 0
            );
            if (!czyMaTeGreNaPlatformieRes.Succeeded) return ServiceResult<TabelkaDruzynResDto>.Fail(czyMaTeGreNaPlatformieRes.StatusCode, czyMaTeGreNaPlatformieRes.Errors);
            if(!czyMaTeGreNaPlatformieRes.Value) return ServiceResult<TabelkaDruzynResDto>.BadRequest(new ErrorItem("Nie można szukać drużyn wymagających tej platformy, ponieważ nie posiadasz tej gry na tej platformie"));
        }

        if (req.IdJezyka != null)
        {
            var jezykiRes = await jezykService.GetJezykiProfiluZRownymiLubNizszymiStopniami(idUzytkownika);
            if (!jezykiRes.Succeeded) return ServiceResult<TabelkaDruzynResDto>.Fail(jezykiRes.StatusCode, jezykiRes.Errors);

            var jezykISopien = jezykiRes.Value.FirstOrDefault(x => x.Jezyk.Id == req.IdJezyka);
            if(jezykISopien == null) return ServiceResult<TabelkaDruzynResDto>.BadRequest(new ErrorItem("Nie posiadasz wymaganego języka, aby szukać drużyn wymagających tego języka"));
            
            if (req.IdStopnia != null)
            {
                var stopien = jezykISopien.Stopnie.FirstOrDefault(x => x.Id == req.IdStopnia);
                if (stopien == null) return ServiceResult<TabelkaDruzynResDto>.BadRequest(new ErrorItem("Nie posiadasz wymaganego stopnia biegłości języka, aby szukać drużyn wymagających tego stopnia biegłości języka"));
            }

        }
        else if (req.IdStopnia != null) return ServiceResult<TabelkaDruzynResDto>.BadRequest(new ErrorItem("Nie można podać stopnia biegłości języka bez podania języka"));

        try
        {
            if(req.IdNastrojuRozgrywki != null)
            {
                var nastroj = await druzynyRepository.GetNastrojRozgrywki(req.IdNastrojuRozgrywki ?? 1); // tylko po to, aby wywaliło błąd gdy nie znajdzie
            } 
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<TabelkaDruzynResDto>.NotFound(new ErrorItem(e.Message));
        }
        var roleGry = await statystykiService.GetRoleGry(req.IdGry); 
        if (!roleGry.Succeeded) return ServiceResult<TabelkaDruzynResDto>.Fail(roleGry.StatusCode, roleGry.Errors);
        
        // sprawdzamy, czy podał poprawne role
        if (req.IdRol.Length > 0)
        {
            var roleGryIds = roleGry.Value.Select(x => x.Id).ToList();
            var nieprawidloweRole = req.IdRol.Where(x => !roleGryIds.Contains(x)).ToList();
            if (nieprawidloweRole.Count > 0) 
                return ServiceResult<TabelkaDruzynResDto>.BadRequest(new ErrorItem(
                    $"Podane role o id: [{string.Join(", ", nieprawidloweRole)}], które zostały podane w wymaganiach drużyny, nie istnieją w bazie danych dla tej gry."
                ));
            
        }
        if(req.IdRol.Length == 0 && roleGry.Value.Count > 0) return ServiceResult<TabelkaDruzynResDto>.BadRequest(new ErrorItem("Jeżeli gra ma role, to należy podać id przynajmniej jednej roli"));
        
        // wszystko powinno być git, można szukać drużyn
        var idDruzyn = await druzynyRepository.WyszukajIdDruzyn(req, idUzytkownika);
        var idDruzynNaStrone = idDruzyn.Take(LiczbaDruzynNaStroneNaStart).ToList();
        var druzynyRes = await GetDruzynyDoTabelki(idDruzynNaStrone.ToArray());
        if(!druzynyRes.Succeeded) return ServiceResult<TabelkaDruzynResDto>.Fail(druzynyRes.StatusCode, druzynyRes.Errors);
        
        return ServiceResult<TabelkaDruzynResDto>.Ok(new TabelkaDruzynResDto(idDruzyn.ToArray(), druzynyRes.Value));
    }
    public async Task<ServiceResult<bool>> DodajUzytkownikaNaMiejsce(int idMiejsca, int idUzytkownika)
    {
        try
        {
            if (idMiejsca <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id miejsca: " + idMiejsca)); 
            if (idUzytkownika <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id użytkownika: " + idUzytkownika)); 
            
            // rozpoczynamy transakcję, aby mieć pewność, że nikt inny nie zajmie tego miejsca w międzyczasie
            var miejsce = await druzynyRepository.GetMiejsceWDruzynie(idMiejsca);
            
            // sprawdzamy, czy miejsce nie jest już zajęte
            if(miejsce.UzytkownikId != null) return ServiceResult<bool>.Conflict(new ErrorItem("To miejsce jest już zajęte"));
            
            // sprawdzamy, czy użytkownik nie należy już do tej drużyny
            if(await druzynyRepository.CzyUzytkownikNalezyDoDruzyny(idUzytkownika, miejsce.DruzynaId)) 
                return ServiceResult<bool>.Conflict(new ErrorItem("Użytkownik już należy do tej drużyny"));
            
            // sprawdzamy, czy użytkownik spełnia wymagania drużyny
            var czyUzytkownikSpelniaWymaganiaDruzynyRes = await CzyUzytkownikSpelniaWymaganiaDruzyny(miejsce.DruzynaId, idUzytkownika);
            if (!czyUzytkownikSpelniaWymaganiaDruzynyRes.Succeeded) return czyUzytkownikSpelniaWymaganiaDruzynyRes;
            if (!czyUzytkownikSpelniaWymaganiaDruzynyRes.Value) return ServiceResult<bool>.Forbidden(new ErrorItem("Nie spełniasz wymagań tego miejsca"));
    
            // sprawdzamy, czy użytkownik spełnia wymagania miejsca
            var czyUzytkownikSpelniaWymaganieMiejscaRes = await CzyUzytkownikSpelniaWymaganieMiejsca(idMiejsca, idUzytkownika);
            if (!czyUzytkownikSpelniaWymaganieMiejscaRes.Succeeded) return czyUzytkownikSpelniaWymaganieMiejscaRes;
            if (!czyUzytkownikSpelniaWymaganieMiejscaRes.Value) return ServiceResult<bool>.Forbidden(new ErrorItem("Nie spełniasz wymagań tego miejsca"));
            
            // sprawdzamy, czy jest prywatna i ma zaproszenie do niej na to miejsce. jeśli nie ma, nie może dołączyć
            var druzyna = await druzynyRepository.GetDruzyna(miejsce.DruzynaId);
            if (!druzyna.CzyPubliczna)
            {
                var powiadomienieRes = await powiadomienieService.CzyUzytkownikMaPowiadomienieDanegoTypuPowiazaneZObiektami(
                    idUzytkownika,
                    (int)TypPowiadomieniaEnum.ZaproszenieDoDruzyny,
                    miejsce.DruzynaId,
                    idMiejsca
                );
                if (!powiadomienieRes.Succeeded) return ServiceResult<bool>.Fail(powiadomienieRes.StatusCode, powiadomienieRes.Errors);
                if (!powiadomienieRes.Value) return ServiceResult<bool>.Forbidden(new ErrorItem("To miejsce jest prywatne i nie masz zaproszenia, aby do niego dołączyć"));
            }
            
            // dodajemy użytkownika na miejsce
            var wynik = await druzynyRepository.DodajUzytkownikaNaMiejsce(idMiejsca, idUzytkownika);
            
            // usuwamy zaproszenia na dane miejsce, bo są nieaktualne
            await powiadomienieService.DeletePowiadomieniaDanegoTypuPowiazaneZObiektami(null, (int)TypPowiadomieniaEnum.ZaproszenieDoDruzyny, miejsce.DruzynaId, idMiejsca);
            
            // jeżeli wynik jest false, to znaczy, że miejsce zajęło ktoś inny w międzyczasie, więc zwracamy konflikt
            if(!wynik) return ServiceResult<bool>.Conflict(new ErrorItem("To miejsce jest już zajęte lub zostało usunięte"));
            
            // wysyłamy powiadomienie do kapitana drużyny, że ktoś do niej dołączył
            var druzynaRes = await GetDruzyna(miejsce.DruzynaId);
            if(!druzynaRes.Succeeded) return ServiceResult<bool>.Fail(druzynaRes.StatusCode, druzynaRes.Errors);
            var rola = miejsce.RolaId != null ? await statystykiService.GetRola(miejsce.RolaId ?? 1) : null;
            var nazwaRoli = rola != null && rola.Succeeded ? rola.Value.Nazwa : null;
            
            await powiadomienieService.WyslijPowiadomienieODolaczeniuDoDruzyny(
                idUzytkownika, 
                druzynaRes.Value.KapitanId, 
                miejsce.DruzynaId, 
                druzynaRes.Value.Nazwa,
                nazwaRoli
            );
            
            return ServiceResult<bool>.NoContent(wynik);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<bool>> ZaprosUzytkownikaNaMiejsce(int idMiejsca, int idZapraszanegoUzytkownika, int idZapraszajacegoUzytkownika)
    {
        if(idMiejsca <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id miejsca: " + idMiejsca));
        if(idZapraszanegoUzytkownika <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id zapraszanego użytkownika: " + idZapraszanegoUzytkownika));
        if(idZapraszajacegoUzytkownika <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id zapraszającego użytkownika: " + idZapraszajacegoUzytkownika));
        
        try
        {
            var miejsce = await druzynyRepository.GetMiejsceWDruzynie(idMiejsca);
            // sprawdzamy, czy miejsce jest zajęte
            if (miejsce.UzytkownikId != null) return ServiceResult<bool>.Conflict(new ErrorItem("To miejsce jest już zajęte"));
            
            var druzyna = await druzynyRepository.GetDruzyna(miejsce.DruzynaId);
            // sprawdzamy, czy zapraszający jest kapitanem drużyny
            if (druzyna.KapitanId != idZapraszajacegoUzytkownika)
                return ServiceResult<bool>.Forbidden(new ErrorItem("Tylko kapitan drużyny może zapraszać użytkowników do drużyny"));
            
            // sprawdzamy, czy zapraszany użytkownik nie należy już do tej drużyny
            if (await druzynyRepository.CzyUzytkownikNalezyDoDruzyny(idZapraszanegoUzytkownika, miejsce.DruzynaId))
                return ServiceResult<bool>.Conflict(new ErrorItem("Użytkownik już należy do tej drużyny"));
            
            
            // sprawdzamy, czy zapraszany użytkownik spełnia wymagania drużyny
            var czyUzytkownikSpelniaWymaganiaDruzynyRes = await CzyUzytkownikSpelniaWymaganiaDruzyny(miejsce.DruzynaId, idZapraszanegoUzytkownika);
            if (!czyUzytkownikSpelniaWymaganiaDruzynyRes.Succeeded) return czyUzytkownikSpelniaWymaganiaDruzynyRes;
            if (!czyUzytkownikSpelniaWymaganiaDruzynyRes.Value)
                return ServiceResult<bool>.Forbidden(new ErrorItem("Zapraszany użytkownik nie spełnia wymagań drużyny"));
            
            // sprawdzamy, czy zapraszany użytkownik spełnia wymagania miejsca
            var czyUzytkownikSpelniaWymaganieMiejscaRes = await CzyUzytkownikSpelniaWymaganieMiejsca(idMiejsca, idZapraszanegoUzytkownika);
            if (!czyUzytkownikSpelniaWymaganieMiejscaRes.Succeeded) return czyUzytkownikSpelniaWymaganieMiejscaRes;
            if (!czyUzytkownikSpelniaWymaganieMiejscaRes.Value)
                return ServiceResult<bool>.Forbidden(new ErrorItem("Zapraszany użytkownik nie spełnia wymagań tego miejsca"));
            
            var rola = miejsce.RolaId != null ? await statystykiService.GetRola(miejsce.RolaId ?? 1) : null;
            var nazwaRoli = rola != null && rola.Succeeded ? rola.Value.Nazwa : null;
            
            // wszystko powinno być git, wysyłamy zaproszenie
            var wynik = await powiadomienieService.WyslijZaproszenieNaMiejsceWDruzynie(
                idZapraszanegoUzytkownika,
                miejsce.DruzynaId,
                druzyna.Nazwa,
                idMiejsca, 
                nazwaRoli
            );
            if(!wynik.Succeeded) return ServiceResult<bool>.Fail(wynik.StatusCode, wynik.Errors);
            return ServiceResult<bool>.NoContent(true);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
}
