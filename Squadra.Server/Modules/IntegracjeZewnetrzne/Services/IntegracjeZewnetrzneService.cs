using Microsoft.AspNetCore.Identity;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.BibliotekaGier.Models;
using Squadra.Server.Modules.BibliotekaGier.Services;
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
    IPlatformaService platformaService,
    IUzytkownikService uzytkownikService) : IIntegracjeZewnetrzneService
{
    
    public async Task<ServiceResult<bool>> ZintegrujKonto(int idUzytkownika, string login, string haslo)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik == null)
            return ServiceResult<bool>.NotFound(new ErrorItem("Nie znaleziono użytkownika o id: " + idUzytkownika));
        
        try
        {
            // hasło haszujemy, bo w bazie zewnętrznego serwisu jest przechowywane zahashowane, więc musimy zahashować to, co użytkownik nam podał, żeby porównać
            var hasher = new PasswordHasher<object>();
            var zahashowaneHaslo = hasher.HashPassword(null!, haslo);
            // tymczasowe, potem usuwamy. to tylko po to, aby przekopiować do bazy
            Console.WriteLine("HASŁO - DO PRZEKOPIOWANIA = " + zahashowaneHaslo);
            
            var idNaZewnetrznymSerwisie = await integracjeZewnetrzneRepository.SprawdzDaneLogowania(login, zahashowaneHaslo);
            if(idNaZewnetrznymSerwisie == null)
                return ServiceResult<bool>.Unauthorized(new ErrorItem("Nieprawidłowy login lub hasło dla zewnętrznego serwisu."));
            var result = await uzytkownikService.UpdateIdNaZewnetrznymSerwisie(idUzytkownika, idNaZewnetrznymSerwisie);
            if (!result.Succeeded)
                return result;
            return ServiceResult<bool>.Ok(true);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
        catch (BladZewnetrznegoSerwisuException e)
        {
            return ServiceResult<bool>.Fail(503,
                new List<ErrorItem> { new("Błąd podczas komunikacji z zewnętrznym serwisem: " + e.Message) });
        }
    }

    // czyścimy jego dane z zewnętrznego serwisu oraz jego id na zewnętrznym serwisie
    public async Task<ServiceResult<bool>> PrzerwijIntegracjeUzytkownika(int idUzytkownika)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik == null)
            return ServiceResult<bool>.NotFound(new ErrorItem("Nie znaleziono użytkownika o id: " + idUzytkownika));
        
        var idNaZewnetrznymSerwisie = uzytkownik.IdNaZewnetrznymSerwisie;
        if (idNaZewnetrznymSerwisie == null)
            return ServiceResult<bool>.BadRequest(new ErrorItem("Użytkownik o id: " + idUzytkownika + " nie jest połączony z zewnętrznym serwisem."));
        
        var transakcja = await context.Database.BeginTransactionAsync();

        var wyczyscDaneResult = await WyczyscCaleZintegrowaneDaneUzytkownika(idUzytkownika);
        if (!wyczyscDaneResult.Succeeded)
        {
            await transakcja.RollbackAsync();
            return wyczyscDaneResult;
        }
        
        var result = await uzytkownikService.UpdateIdNaZewnetrznymSerwisie(idUzytkownika, idNaZewnetrznymSerwisie);
        if (!result.Succeeded)
        {
            await transakcja.RollbackAsync();
            return result;
        }
        
        await transakcja.CommitAsync();

        return ServiceResult<bool>.Ok(true);

        // przydałoby się jeszcze przejść po drużynach i gildiach i wyrzucić go z każdej, która ma wymagania co do statystyk
    }
    
    
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
    
    private async Task<ServiceResult<bool>> WyczyscCaleZintegrowaneDaneUzytkownika(int idUzytkownika)
    {
        
        var transakcja = await context.Database.BeginTransactionAsync();
        
        var wyczyscBibliotekeResult = await bibliotekaGierService.WyczyscBibliotekeUzytkownika(idUzytkownika);
        if (!wyczyscBibliotekeResult.Succeeded)
        {
            await transakcja.RollbackAsync();
            return wyczyscBibliotekeResult;
        }

        var wyczyscPlatformyResult = await platformaService.UsunPlatformyUzytkownika(idUzytkownika);
        if (!wyczyscPlatformyResult.Succeeded)
        {
            await transakcja.RollbackAsync();
            return wyczyscPlatformyResult;
        }

        var wyczyscStatystykiResult = await statystykiService.UsunStatystykiUzytkownika(idUzytkownika);
        if (!wyczyscStatystykiResult.Succeeded)
        {
            await transakcja.RollbackAsync();
            return wyczyscStatystykiResult;
        }
        
        await transakcja.CommitAsync();

        return ServiceResult<bool>.Ok(true);
    }
    
    private async Task<ServiceResult<bool>> UpdateBibliotekeGierUzytkownika(int idUzytkownika, int idNaZewnetrznymSerwisie)
    {
        try
        {
            var zewnetrzneGryNaPlatformie =
                await integracjeZewnetrzneRepository.GetGryUzytkownikaNaPlatformie(idNaZewnetrznymSerwisie);
            var gryNaPlatformie = zewnetrzneGryNaPlatformie.Select(g => new GraUzytkownikaNaPlatformie
            {
                GraId = g.IdGry,
                PlatformaId = g.IdPlatformy
            }).ToList();

            var zewnetrzneGry = await integracjeZewnetrzneRepository.GetGryUzytkownika(idNaZewnetrznymSerwisie);
            var gry = zewnetrzneGry.Select(g => new GraUzytkownika
            {
                GraId = g.IdGry,
                UzytkownikId = idUzytkownika
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
    
    private async Task<ServiceResult<bool>> UpdatePlatformyUzytkownika(int idUzytkownika, int idNaZewnetrznymSerwisie)
    {
        try{
            var zewnetrznePlatformy = await integracjeZewnetrzneRepository.GetPlatformyUzytkownika(idNaZewnetrznymSerwisie);
            var platformy = zewnetrznePlatformy.Select(p => new UzytkownikPlatforma
            {
                PlatformaId = p.PlatformaId,
                PseudonimNaPlatformie = p.PseudonimNaPlatformie
            }).ToList();

            var wynik = await platformaService.UpdatePlatformyUzytkownika(idUzytkownika, platformy);
            return wynik;
        }
        catch (BladZewnetrznegoSerwisuException e)
        {
            return ServiceResult<bool>.Fail(503,
                new List<ErrorItem> { new("Błąd podczas komunikacji z zewnętrznym serwisem: " + e.Message) });
        }
    }

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