using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.BibliotekaGier.Models;
using Squadra.Server.Modules.BibliotekaGier.Services;
using Squadra.Server.Modules.Drużyny.Services;
using Squadra.Server.Modules.IntegracjeZewnetrzne.DTO;
using Squadra.Server.Modules.IntegracjeZewnetrzne.Repositories;
using Squadra.Server.Modules.Platformy.Models;
using Squadra.Server.Modules.Platformy.Services;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Statystyki.Models;
using Squadra.Server.Modules.Statystyki.Services;
using Squadra.Server.Modules.Uzytkownicy.Services;

namespace Squadra.Server.Modules.IntegracjeZewnetrzne.Services;

public class IntegracjeZewnetrzneService(
    AppDbContext context,
    IIntegracjeZewnetrzneRepository integracjeZewnetrzneRepository,
    IStatystykiService statystykiService,
    IBibliotekaGierService bibliotekaGierService,
    IPlatformyService platformyService,
    IDruzynyService druzynyService,
    IUzytkownicyService uzytkownicyService) : IIntegracjeZewnetrzneService
{
    
    // symulujemy logowanie do zewnętrznego serwisu. jeżeli dopasuje podane dane do któregoś z "kont", przydziela je do konta obecnego użytkownika
    // po tym pobiera od razu dane dla tamtego konta, aby użytkownik nie musiał czekać na automatyczną aktualizację
    public async Task<ServiceResult<ZintegrujKontoRes>> ZintegrujKonto(int idUzytkownika, string login, string haslo)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik == null)
            return ServiceResult<ZintegrujKontoRes>.NotFound(new ErrorItem("Nie znaleziono użytkownika o id: " + idUzytkownika));
        
        // sprawdzamy, czy użytkownik nie jest już połączony z zewnętrznym serwisem, jeśli tak to zwracamy błąd
        if(uzytkownik.IdNaZewnetrznymSerwisie != null)
           return ServiceResult<ZintegrujKontoRes>.BadRequest(new ErrorItem("Użytkownik o id: " + idUzytkownika + " jest już połączony z zewnętrznym serwisem. Przerwij integrację, aby połączyć inne konto."));
        
        // sprawdzamy, czy ten login nie jest już zajęty
        if(await context.Uzytkownik.AnyAsync(x => x.LoginNaZewnetrznymSerwisie != null && x.LoginNaZewnetrznymSerwisie.Equals(login.ToLower())))
            return ServiceResult<ZintegrujKontoRes>.BadRequest(new ErrorItem("Podany login jest już połączony z innym użytkownikiem."));
        try
        {
            // porównujemy dane
            var hasher = new PasswordHasher<object>();
            var daneKonta = await integracjeZewnetrzneRepository.ZwrocDaneKonta(login.ToLower());
            if(daneKonta == null)
                return ServiceResult<ZintegrujKontoRes>.Unauthorized(new ErrorItem("Nieprawidłowy login lub hasło dla zewnętrznego serwisu."));
            var hasloZewnetrznegoSerwisu = daneKonta.HasloHash;
            var weryfikacjaHasla = hasher.VerifyHashedPassword(null!, hasloZewnetrznegoSerwisu, haslo);
            if (weryfikacjaHasla == PasswordVerificationResult.Failed)
                return ServiceResult<ZintegrujKontoRes>.Unauthorized(new ErrorItem("Nieprawidłowy login lub hasło dla zewnętrznego serwisu."));
            
            // aktualizujemy login i id na zewnętrznym serwisie w bazie danych
            var result = await uzytkownicyService.UpdateDaneKontaNaZewnetrznymSerwisie(idUzytkownika, daneKonta.Id, login);
            if (!result.Succeeded)
                return ServiceResult<ZintegrujKontoRes>.Fail(result.StatusCode, result.Errors);
            
            // aktualizujemy zintegrowane dane użytkownika, czyli jego gry, platformy i statystyki
            var result2 = await UpdateCaleZintegrowaneDaneUzytkownika(idUzytkownika);
            if (!result2.Succeeded)
                return ServiceResult<ZintegrujKontoRes>.Fail(result2.StatusCode, result2.Errors);
            
            return ServiceResult<ZintegrujKontoRes>.Ok(new ZintegrujKontoRes(daneKonta.Id, login));
        }
        catch (NieZnalezionoWBazieException e)  
        {
            return ServiceResult<ZintegrujKontoRes>.NotFound(new ErrorItem(e.Message));
        }
        catch (BladZewnetrznegoSerwisuException e)
        {
            return ServiceResult<ZintegrujKontoRes>.Fail(503,
                new List<ErrorItem> { new("Błąd podczas komunikacji z zewnętrznym serwisem: " + e.Message) });
        }
    }

    // czyścimy jego dane z zewnętrznego serwisu oraz jego id na zewnętrznym serwisie
    public async Task<ServiceResult<bool>> PrzerwijIntegracjeUzytkownika(int idUzytkownika, bool czyPrzyUsuwaniuKonta = false)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik == null)
            return ServiceResult<bool>.NotFound(new ErrorItem("Nie znaleziono użytkownika o id: " + idUzytkownika));
        
        var idNaZewnetrznymSerwisie = uzytkownik.IdNaZewnetrznymSerwisie;
        if (idNaZewnetrznymSerwisie == null)
        {
            if(czyPrzyUsuwaniuKonta) return ServiceResult<bool>.Ok(true);
            return ServiceResult<bool>.BadRequest(new ErrorItem("Użytkownik o id: " + idUzytkownika + " nie jest połączony z zewnętrznym serwisem."));
        }
        
        // Jeśli transakcja już istnieje (np. z UsunKonto), nie otwieramy nowej
        var czyToNowaTransakcja = context.Database.CurrentTransaction == null;
        var transakcja = czyToNowaTransakcja 
            ? await context.Database.BeginTransactionAsync() 
            : null;

        var wyczyscDaneResult = await WyczyscCaleZintegrowaneDaneUzytkownika(idUzytkownika);
        if (!wyczyscDaneResult.Succeeded)
        {
            if (czyToNowaTransakcja)
                await transakcja!.RollbackAsync();
            return wyczyscDaneResult;
        }
        
        // wyrzucamy go z drużyn używających zintegrowanych danych i usuwamy jego zintegrowane drużyny
        var druzynyResult = await druzynyService.PrzerwijIntegracjeUzytkownikaOdnosnieDruzyn(idUzytkownika);
        if (!druzynyResult.Succeeded)
        {
            if (czyToNowaTransakcja)
                await transakcja!.RollbackAsync();
            return druzynyResult;
        }
        
        // czyścimy jego id i login na zewnętrznym serwisie
        var result = await uzytkownicyService.UpdateDaneKontaNaZewnetrznymSerwisie(idUzytkownika, null, null);
        if (!result.Succeeded)
        {
            if (czyToNowaTransakcja)
                await transakcja!.RollbackAsync();
            return result;
        }
        
        if (czyToNowaTransakcja)
            await transakcja!.CommitAsync();

        return ServiceResult<bool>.Ok(true);
    }
    
    // jednorazowo, przy "łączeniu kont", pobieramy z serwisu dane, aby uzytkownik nie musiał czekać na automatyczną aktualizację
    public async Task<ServiceResult<bool>> UpdateCaleZintegrowaneDaneUzytkownika(int idUzytkownika)
    {
        
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik == null)
            return ServiceResult<bool>.NotFound(new ErrorItem("Nie znaleziono użytkownika o id: " + idUzytkownika));
        
        var idNaZewnetrznymSerwisie = uzytkownik.IdNaZewnetrznymSerwisie;
        if (idNaZewnetrznymSerwisie == null)
            return ServiceResult<bool>.BadRequest(new ErrorItem("Użytkownik o id: " + idUzytkownika + " nie ma przypisanego id na zewnętrznym serwisie."));

        var transakcja = await context.Database.BeginTransactionAsync();
        
        var updateBibliotekaGierResult = await UpdateBibliotekeGierUzytkownika(idUzytkownika, idNaZewnetrznymSerwisie.Value);
        if (!updateBibliotekaGierResult.Succeeded)
        {
            await transakcja.RollbackAsync();
            return updateBibliotekaGierResult;
        }

        var updatePlatformyResult = await UpdatePlatformyUzytkownika(idUzytkownika, idNaZewnetrznymSerwisie.Value);
        if (!updatePlatformyResult.Succeeded)
        {
            await transakcja.RollbackAsync();
            return updatePlatformyResult;
        }

        var updateStatystykiResult = await UpdateStatystykiUzytkownika(idUzytkownika, idNaZewnetrznymSerwisie.Value);
        if (!updateStatystykiResult.Succeeded)
        {
            await transakcja.RollbackAsync();
            return updateStatystykiResult;
        }
        
        await transakcja.CommitAsync();

        return ServiceResult<bool>.Ok(true);
    }
    
    // przy kończeniu integracji użytkownika usuwamy jego biblitekę gier, platformy i statystyki 
    private async Task<ServiceResult<bool>> WyczyscCaleZintegrowaneDaneUzytkownika(int idUzytkownika)
    {
        var wyczyscBibliotekeResult = await bibliotekaGierService.WyczyscBibliotekeUzytkownika(idUzytkownika);
        if (!wyczyscBibliotekeResult.Succeeded)
        {
            return wyczyscBibliotekeResult;
        }

        var wyczyscPlatformyResult = await platformyService.UsunPlatformyUzytkownika(idUzytkownika);
        if (!wyczyscPlatformyResult.Succeeded)
        {
            return wyczyscPlatformyResult;
        }

        var wyczyscStatystykiResult = await statystykiService.UsunStatystykiUzytkownika(idUzytkownika);
        if (!wyczyscStatystykiResult.Succeeded)
        {
            return wyczyscStatystykiResult;
        }
        
        return ServiceResult<bool>.Ok(true);
    }
    
    // zastępujemy gry w bibliotece użytkownika grami pobranymi z zewnętrznego serwisu
    private async Task<ServiceResult<bool>> UpdateBibliotekeGierUzytkownika(int idUzytkownika, int idNaZewnetrznymSerwisie)
    {
        try
        {
            var zewnetrzneGry = await integracjeZewnetrzneRepository.GetGryUzytkownika(idNaZewnetrznymSerwisie);
            var gry = zewnetrzneGry.Select(g => new GraUzytkownika
            {
                GraId = g.IdGry,
                UzytkownikId = idUzytkownika
            }).ToList();
            
            var zewnetrzneGryNaPlatformie =
                await integracjeZewnetrzneRepository.GetGryUzytkownikaNaPlatformie(idNaZewnetrznymSerwisie);
            var gryNaPlatformie = zewnetrzneGryNaPlatformie.Select(g => new GraUzytkownikaNaPlatformie
            {
                GraId = g.IdGry,
                PlatformaId = g.IdPlatformy
            }).ToList();

            var wynik = await bibliotekaGierService.UpdateBibliotekeGierUzytkownika(idUzytkownika, gryNaPlatformie,
                gry);
            return wynik;
        }
        catch (BladZewnetrznegoSerwisuException e)
        {
            return ServiceResult<bool>.Fail(503,
                new List<ErrorItem> { new("Błąd podczas komunikacji z zewnętrznym serwisem: " + e.Message) });
        }
    }
    
    // zastępujemy platformy użytkownika platformami pobranymi z zewnętrznego serwisu
    private async Task<ServiceResult<bool>> UpdatePlatformyUzytkownika(int idUzytkownika, int idNaZewnetrznymSerwisie)
    {
        try{
            var zewnetrznePlatformy = await integracjeZewnetrzneRepository.GetPlatformyUzytkownika(idNaZewnetrznymSerwisie);
            var platformy = zewnetrznePlatformy.Select(p => new UzytkownikPlatforma
            {
                UzytkownikId = idUzytkownika,
                PlatformaId = p.PlatformaId,
            }).ToList();

            var wynik = await platformyService.UpdatePlatformyUzytkownika(idUzytkownika, platformy);
            return wynik;
        }
        catch (BladZewnetrznegoSerwisuException e)
        {
            return ServiceResult<bool>.Fail(503,
                new List<ErrorItem> { new("Błąd podczas komunikacji z zewnętrznym serwisem: " + e.Message) });
        }
    }

    // zastępujemy statystyki użytkownika statystykami pobranymi z zewnętrznego serwisu
    private async Task<ServiceResult<bool>> UpdateStatystykiUzytkownika(int idUzytkownika, int idNaZewnetrznymSerwisie)
    {
        try
        {
            var zewnetrzneStatystyki =
                await integracjeZewnetrzneRepository.GetStatystykiUzytkownika(idNaZewnetrznymSerwisie);
            var statystyki = zewnetrzneStatystyki.Select(s => new StatystykaUzytkownika
            {
                StatystykaId = s.StatystykaId,
                UzytkownikId = idUzytkownika,
                Wartosc = s.Wartosc,
                PorownywalnaWartoscLiczbowa = s.PorownywalnaWartoscLiczbowa
            }).ToList();

            var wynik = await statystykiService.UpdateStatystykiUzytkownika(idUzytkownika, statystyki);
            return wynik;
        }
        catch (BladZewnetrznegoSerwisuException e)
        {
            return ServiceResult<bool>.Fail(503,
                new List<ErrorItem> { new("Błąd podczas komunikacji z zewnętrznym serwisem: " + e.Message) });
        }
    }
}