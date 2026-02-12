using Squadra.Server.Exceptions;
using Squadra.Server.Models;
using Squadra.Server.Modules.Znajomi.DTO;
using Squadra.Server.Repositories;

namespace Squadra.Server.Services;

public class ZnajomiService(
    IZnajomiRepository znajomiRepository,
    IProfilService profilService, 
    IStatystykiCzatuService statystykiCzatuService) : IZnajomiService
{

    public async Task<ServiceResult<ICollection<Znajomi>>> GetZnajomiUzytkownika(int id)
    {
        if(id < 1) return ServiceResult<ICollection<Znajomi>>.NotFound(new ErrorItem("Użytkownik o id " + id + " nie istnieje"));
        
        try
        {
            return ServiceResult<ICollection<Znajomi>>.Ok(await znajomiRepository.GetZnajomiUzytkownika(id));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<ICollection<Znajomi>>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    // zwracamy listę znajomych do pokazania na stronie "moi znajomi", potrzebujemy pseudonimu, awatara, nazwy statusu,
    // daty najnowszej wiadomości między nimi (do sortowania) i informacji, czy są jakieś nowe wiadomości od tego znajomego
    public async Task<ServiceResult<ICollection<ZnajomyDoListyDto>>> GetZnajomiDoListyUzytkownika(int id)
    {
        // najpierw z bazy znajomych pobieramy id wszystkich osób, które są w parze z naszym
        ICollection<Znajomi> znajomi = await znajomiRepository.GetZnajomiUzytkownika(id);
        List<ZnajomyDoListyDto> listaDoZwrocenia = new List<ZnajomyDoListyDto>();
        // dla każdego użytkownika bierzemy jego profil
        foreach (var uzytkownik in znajomi)
        {
            var idZnajomego = uzytkownik.IdUzytkownika1 == id ? uzytkownik.IdUzytkownika2 : uzytkownik.IdUzytkownika1;
            
            // bierzemy profil znajomego, potrzebujemy z niego pseudonim, awatar i nazwę statusu
            var profilRes = await profilService.GetProfil(idZnajomego);
            
            var profil = profilRes.Value;
            
            if(profil == null) return ServiceResult<ICollection<ZnajomyDoListyDto>>.NotFound(profilRes.Errors[0]);
            
            // bierzemy datę najnowszej wiadomości między nimi, potrzebujemy jej do posortowania znajomych
            var dataNajnowszejWiadomosciRes = await statystykiCzatuService.GetDataNajnowszejWiadomosci(id, idZnajomego);
            if(dataNajnowszejWiadomosciRes.StatusCode == 404) return ServiceResult<ICollection<ZnajomyDoListyDto>>.NotFound(dataNajnowszejWiadomosciRes.Errors[0]);
            if(dataNajnowszejWiadomosciRes.StatusCode == 400) return ServiceResult<ICollection<ZnajomyDoListyDto>>.BadRequest(dataNajnowszejWiadomosciRes.Errors[0]);
            
            var czySaNoweWiadomosciRes = await statystykiCzatuService.CzySaNoweWiadomosciOdZnajomego(id, idZnajomego);
            if(czySaNoweWiadomosciRes.StatusCode == 404) return ServiceResult<ICollection<ZnajomyDoListyDto>>.NotFound(czySaNoweWiadomosciRes.Errors[0]);
            if(czySaNoweWiadomosciRes.StatusCode == 400) return ServiceResult<ICollection<ZnajomyDoListyDto>>.BadRequest(czySaNoweWiadomosciRes.Errors[0]);
    
            var znajomyDoListy = new ZnajomyDoListyDto(
                    profil.Pseudonim,
                    profil.Awatar ?? [],
                    dataNajnowszejWiadomosciRes.Value,
                    profil.NazwaStatusu,
                    czySaNoweWiadomosciRes.Value
            );
            listaDoZwrocenia.Add(znajomyDoListy);
        }
        
        // sortujemy znajomych po dacie najnowszej wiadomości, żeby ci, z którymi ostatnio rozmawialiśmy, byli na górze listy. Ci, którzy nie mają żadnych wiadomości, będą na dole listy, posortowani alfabetycznie po pseudonimie
        listaDoZwrocenia = listaDoZwrocenia.OrderByDescending(x => x.DataOstatniejWiadomosci).ThenBy(x => x.Pseudonim).ToList();
        
        return ServiceResult<ICollection<ZnajomyDoListyDto>>.Ok(listaDoZwrocenia);
    }
    
    // zwracamy datę ostatniego otwarcia czatu między dwoma użytkownikami, potrzebujemy jej do sprawdzenia, czy są jakieś nowe wiadomości od tego znajomego
    public async Task<ServiceResult<DateTime?>> GetDataOstatniegoOtwarciaCzatu(int idSprawdzajacego, int idZnajomego)
    {
        try
        {
            if (idSprawdzajacego < 1) return ServiceResult<DateTime?>.NotFound(new ErrorItem("Uzytkownik o id " + idSprawdzajacego + " nie istnieje"));
            if (idZnajomego < 1) return ServiceResult<DateTime?>.NotFound(new ErrorItem("Uzytkownik o id " + idZnajomego + " nie istnieje"));
            
            return ServiceResult<DateTime?>.Ok(await znajomiRepository.GetDataOstatniegoOtwarciaCzatu(idSprawdzajacego, idZnajomego));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<DateTime?>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<bool>> CreateZnajomosc(int idUzytkownika1, int idUzytkownika2)
    {
        try
        {
            if (idUzytkownika1 < 1) return ServiceResult<bool>.NotFound(new ErrorItem("Uzytkownik o id " + idUzytkownika1 + " nie istnieje"));
            if (idUzytkownika2 < 1) return ServiceResult<bool>.NotFound(new ErrorItem("Uzytkownik o id " + idUzytkownika2 + " nie istnieje"));
            
            if(idUzytkownika1 == idUzytkownika2)
                return ServiceResult<bool>.BadRequest(new ErrorItem("Użytkownik nie może dodać siebie do znajomych"));
            
            // trzeba tutaj oddzielnie sprawdzić, czy żaden użytkownik nie przekroczył maksymalnej liczby znajomych
            // bo powiadomienia mogą być wysyłane asynchronicznie i wtedy może się okazać, że obaj użytkownicy przekroczyli limit
            var znajomiUzytkownika1 = await znajomiRepository.GetZnajomiUzytkownika(idUzytkownika1);
            if (znajomiUzytkownika1.Count >= MaxLiczbaZnajomych)
                return ServiceResult<bool>.BadRequest(new ErrorItem("Uzytkownik o id " + idUzytkownika1 + " osiągnął maksymalną liczbę znajomych: " + MaxLiczbaZnajomych));
            var znajomiUzytkownika2 = await znajomiRepository.GetZnajomiUzytkownika(idUzytkownika2);
            if (znajomiUzytkownika2.Count >= MaxLiczbaZnajomych)
                return ServiceResult<bool>.BadRequest(new ErrorItem("Uzytkownik o id " + idUzytkownika2 + " osiągnął maksymalną liczbę znajomych: " + MaxLiczbaZnajomych));
            

            return ServiceResult<bool>.Created(await znajomiRepository.CreateZnajomosc(idUzytkownika1, idUzytkownika2));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<bool>> DeleteZnajomosc(int idUzytkownikaInicjujacego, int idUzytkownika2)
    {
        try
        {
            if (idUzytkownikaInicjujacego < 1)
                return ServiceResult<bool>.NotFound(
                    new ErrorItem("Uzytkownik o id " + idUzytkownikaInicjujacego + " nie istnieje"));
            if (idUzytkownika2 < 1)
                return ServiceResult<bool>.NotFound(
                    new ErrorItem("Uzytkownik o id " + idUzytkownika2 + " nie istnieje"));
            
            // historię wiadomości usuwa repozytorium, bo tam jest transakcja
            
            return ServiceResult<bool>.NoContent(await znajomiRepository.DeleteZnajomosc(idUzytkownikaInicjujacego, idUzytkownika2));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    // aktualizujemy datę ostatniego otwarcia czatu między dwoma użytkownikami, gdy użytkownik otworzy czat
    public async Task<ServiceResult<bool>> ZaktualizujOstatnieOtwarcieCzatu(int idOtwierajacego, int idZnajomego)
    {
        try
        {
            if (idOtwierajacego < 1) return ServiceResult<bool>.NotFound(new ErrorItem("Uzytkownik o id " + idOtwierajacego + " nie istnieje"));
            if (idZnajomego < 1) return ServiceResult<bool>.NotFound(new ErrorItem("Uzytkownik o id " + idZnajomego + " nie istnieje"));
            
            return ServiceResult<bool>.Ok(await znajomiRepository.ZaktualizujOstatnieOtwarcieCzatu(idOtwierajacego, idZnajomego));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    // maksymalna liczba znajomych jednego użytkownika, statyczna wartość dostępna dla innych serwisów
    public const int MaxLiczbaZnajomych = 100;
}