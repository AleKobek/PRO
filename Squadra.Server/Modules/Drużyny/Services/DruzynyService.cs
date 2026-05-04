using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Drużyny.DTO;
using Squadra.Server.Modules.Drużyny.Repositories;
using Squadra.Server.Modules.Platformy.Services;
using Squadra.Server.Modules.Profile.DTO.Profil;
using Squadra.Server.Modules.Profile.Services;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Statystyki.Services;
using Squadra.Server.Modules.Uzytkownicy.Services;
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
    IPlatformaService platformaService
    ) : IDruzynyService
{
    public async Task<ServiceResult<DruzynaDoTabelkiDto>> GetDruzynaDoTabelki(int idDruzyny)
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
            var minutyOdOstatniejAktywnosciKapitana = (int)(DateTime.UtcNow - ostatniaAktywnoscKapitana).TotalMinutes;
            // zmieniamy to na string w formacie "X dni Y godzin Z minut temu"
            var ostatniaAktywnoscKapitanaString = minutyOdOstatniejAktywnosciKapitana switch
            {
                < 2 => "teraz",
                < 5 => $"{minutyOdOstatniejAktywnosciKapitana} minuty temu",
                < 60 => $"{minutyOdOstatniejAktywnosciKapitana} minut temu",
                < 60 * 24 => $"{minutyOdOstatniejAktywnosciKapitana / 60} godzin temu",
                _ => $"{minutyOdOstatniejAktywnosciKapitana / (60 * 24)} dni temu"
            };

            var czlonkowieDruzynyRes = await GetCzlonkowieDruzyny(idDruzyny);
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
    
    public async Task<ServiceResult<DruzynaSzczegolyDto>> PodajSzczegolyDruzyny(int idDruzyny)
    {
        if(idDruzyny <= 0) return ServiceResult<DruzynaSzczegolyDto>.BadRequest(new ErrorItem("Id drużyny musi być większe od 0"));
        var druzyna = await druzynyRepository.GetDruzyna(idDruzyny);
        
        // pobieramy grę, żeby mieć jej tytuł
        var graRes = await wspieranaGraService.GetWspieranaGra(druzyna.GraId);
        if (!graRes.Succeeded) return ServiceResult<DruzynaSzczegolyDto>.Fail(graRes.StatusCode, graRes.Errors);
        
        // pobieramy członków drużyny
        var czlonkowieDruzynyRes = await GetCzlonkowieDruzyny(idDruzyny);
        if (!czlonkowieDruzynyRes.Succeeded) return ServiceResult<DruzynaSzczegolyDto>.Fail(czlonkowieDruzynyRes.StatusCode, czlonkowieDruzynyRes.Errors);
        var czlonkowieDruzyny = czlonkowieDruzynyRes.Value ?? new List<MiejsceWDruzynieSzczegolyDto>();
        
        // pobieramy nastrój rozgrywki
        var nastrojRozgrywki = await druzynyRepository.GetNastrojRozgrywki(druzyna.NastrojRozgrywkiId);
        
        // pobieramy wymageny język i stopień biegłości, żeby mieć ich nazwy
        var jezykRes = druzyna.WymaganyJezykId != null 
            ? await jezykService.GetJezyk(druzyna.WymaganyJezykId ?? 0) // już odfiltrowaliśmy drużyny bez WymaganyJezykId, więc możemy bezpiecznie użyć ?? 0
            : null;
        
        if (jezykRes is { Succeeded: false }) return ServiceResult<DruzynaSzczegolyDto>.Fail(jezykRes.StatusCode, jezykRes.Errors);
        
        var stopienBieglosciRes = druzyna.WymaganyStopienBieglosciJezykaId != null
            ? await stopienBieglosciJezykaService.GetStopienBieglosciJezyka(druzyna.WymaganyStopienBieglosciJezykaId ?? 0) // już odfiltrowaliśmy drużyny bez WymaganyStopienBieglosciJezykaId, więc możemy bezpiecznie użyć ?? 0
            : null;
        
        if (stopienBieglosciRes is { Succeeded: false }) return ServiceResult<DruzynaSzczegolyDto>.Fail(stopienBieglosciRes.StatusCode, stopienBieglosciRes.Errors);

        
        var jezykIStopienBiegłosci = jezykRes != null && stopienBieglosciRes != null
            ? $"{jezykRes.Value.Nazwa} - {stopienBieglosciRes.Value.Nazwa}" // już odfiltrowaliśmy drużyny bez WymaganyJezykId i WymaganyStopienBieglosciJezykaId, więc możemy bezpiecznie użyć .Value
            : jezykRes.Value?.Nazwa; 
        
        // pobieramy wymagania drużyny
        var wymaganiaRes = await statystykiService.GetWymaganiaDruzynyDoWyswietlenia(idDruzyny);
        if (!wymaganiaRes.Succeeded) return ServiceResult<DruzynaSzczegolyDto>.Fail(wymaganiaRes.StatusCode, wymaganiaRes.Errors);
        
        // pobieramy platformę, żeby mieć jej nazwę i logo
        string? nazwaPlatformy = null;
        byte[]? logoPlatformy = null;
        if (druzyna.PlatformaId != null)
        {
            var platformaRes = await platformaService.GetPlatforma(druzyna.PlatformaId ?? 0); // już odfiltrowaliśmy drużyny bez PlatformaId, więc możemy bezpiecznie użyć ?? 0
            if (!platformaRes.Succeeded) return ServiceResult<DruzynaSzczegolyDto>.Fail(platformaRes.StatusCode, platformaRes.Errors);
            nazwaPlatformy = platformaRes.Value.Nazwa; // już odfiltrowaliśmy drużyny bez PlatformaId, więc możemy bezpiecznie użyć .Value
            logoPlatformy = platformaRes.Value.Logo;
        }
       
        // składamy wszystko do kupy i zwracamy szczegóły drużyny
        return ServiceResult<DruzynaSzczegolyDto>.Ok(new DruzynaSzczegolyDto(
            druzyna.Nazwa,
            graRes.Value.Tytul, // jeżeli się powiodło, to Value nie jest null, więc można bezpiecznie użyć .Value
            druzyna.Opis,
            nastrojRozgrywki.Nazwa,
            czlonkowieDruzyny
                .Select(x => new MiejsceWDruzynieSzczegolyDto(x.Czlonek, x.Rola, x.Wymaganie, x.CzyKapitan))
                .ToList(),
            jezykIStopienBiegłosci,
            wymaganiaRes.Value, // już się upewniliśmy, że wymaganiaRes.Value nie jest null, więc można bezpiecznie użyć .Value
            druzyna.CzyPubliczna,
            druzyna.Czy18Plus,
            nazwaPlatformy,
            logoPlatformy
        ));
    }
    
    public async Task<ServiceResult<ICollection<MiejsceWDruzynieSzczegolyDto>>> GetCzlonkowieDruzyny(int idDruzyny)
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
                czlonek,
                miejsce.Rola?.Nazwa,
                wymaganie,
                druzyna.KapitanId == miejsce.UzytkownikId
            ));
        }

        return ServiceResult<ICollection<MiejsceWDruzynieSzczegolyDto>>.Ok(czlonkowieDoZwrocenia);
    }

}