using Microsoft.EntityFrameworkCore.Storage;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Drużyny.Repositories;
using Squadra.Server.Modules.Powiadomienia.Services;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Statystyki.Services;
using Squadra.Server.Modules.Wiadomosci.Services;

namespace Squadra.Server.Modules.Drużyny.Services;

public class DeleteDruzynaService(
    AppDbContext context,
    IDruzynyRepository druzynyRepository,
    IDruzynyService druzynyService,
    IStatystykiService statystykiService,
    IPowiadomienieService powiadomienieService,
    IWiadomoscService wiadomoscService
) : IDeleteDruzynaService
{
    // usuwa drużynę, wraz z miejscami w niej i wymaganymi statystykami. wysyła też powiadomienia o rozwiązaniu drużyny do byłych członków
    public async Task<ServiceResult<bool>> UsunDruzyne(int idDruzyny, int idUsuwajacegoUzytkownika)
    {
        IDbContextTransaction? transakcja = context.Database.CurrentTransaction;
        var createdTransaction = false;
        if (transakcja == null)
        {
            transakcja = await context.Database.BeginTransactionAsync();
            createdTransaction = true;
        }
        try
        {
            if (idDruzyny <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id drużyny: " + idDruzyny));

            var druzyna = await druzynyRepository.GetDruzyna(idDruzyny);
            if (druzyna.KapitanId != idUsuwajacegoUzytkownika)
                return ServiceResult<bool>.Forbidden(new ErrorItem("Tylko kapitan drużyny może ją usunąć"));
            
            var statystykiRes = await statystykiService.UsunWymaganeStatystykiDruzyny(idDruzyny);
            if (!statystykiRes.Succeeded)
            {
                await transakcja.RollbackAsync();
                return statystykiRes;
            }

            // usuwamy wszystkie miejsca w drużynie, żeby nie było miejsc w drużynie, która już nie istnieje
            await druzynyRepository.DeleteMiejscaWDruzynie(idDruzyny);
            
            // usuwamy wszystkie wiadomości na czacie drużyny, żeby nie było wiadomości w drużynie, która już nie istnieje
            var czatDruzynowyRes = await wiadomoscService.DeleteWiadomosciDruzyny(idDruzyny);
            if (!czatDruzynowyRes.Succeeded)
            {
                await transakcja.RollbackAsync();
                return czatDruzynowyRes;
            }


            // usuwamy wszystkie powiadomienia związane z drużyną, żeby nie było powiadomień o drużynie, która już nie istnieje
            // czyli to będą zaproszenia do tej drużyny i u kapitana powiadomienia że ktoś dołączył/opuścił, ktoś odrzucił/zaakceptował zaproszenie .
            var powiadomieniaRes = await powiadomienieService.UsunPowiadomieniaZwiazaneZDruzyna(idDruzyny);
            if (!powiadomieniaRes.Succeeded)
            {
                await transakcja.RollbackAsync();
                return powiadomieniaRes;
            }
            
            // wysyłamy powiadomienia do wszystkich członków drużyny, że drużyna została usunięta
            var czlonkowieDruzyny = await druzynyRepository.GetMiejscaWDruzynie(idDruzyny);
            foreach (var miejsce in czlonkowieDruzyny)
            {
                if (miejsce.UzytkownikId == null) continue; // jeżeli miejsce jest puste, to nie wysyłamy powiadomienia
                if (miejsce.UzytkownikId == idUsuwajacegoUzytkownika)
                    continue; // nie wysyłamy powiadomienia do osoby, która usunęła drużynę
                
                await powiadomienieService.WyslijPowiadomienieORozwiazaniuDruzyny(
                    miejsce.UzytkownikId ??
                    0, // już odfiltrowaliśmy miejsca bez UzytkownikId, więc możemy bezpiecznie użyć ?? 0
                    druzyna.Nazwa
                );
                // nie przerywamy pętli jeżeli wysyłanie powiadomienia się nie powiedzie - coś jest nie tak z miejscem, ale drużyna i tak została usunięta, więc nie ma co robić w tej sytuacji
            } 
            
            // usuwamy drużynę z bazy danych
            await druzynyRepository.UsunDruzyne(idDruzyny);

            if (createdTransaction)
            {
                await transakcja.CommitAsync();
            }

            return ServiceResult<bool>.Ok(true);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
        catch (Exception e)
        {
            if (createdTransaction)
            {
                await transakcja.RollbackAsync();
                Console.WriteLine("Wystąpił błąd podczas usuwania drużyny: " + e.Message);
                return ServiceResult<bool>.Fail(500, [new ErrorItem("Wystąpił błąd podczas usuwania drużyny: " + e.Message)]);
            }
            // jeśli nie utworzyliśmy transakcji tutaj, rzucamy wyjątek dalej aby zewnętrzna transakcja mogła go obsłużyć/rollbackować
            throw;
        }
    }
    
    // funkcja bardzo podobna do "zwykłego" usuwania, ale nie musisz być jej kapitanem i do kapitana również wysyłane jest powiadomienie
    public async Task<ServiceResult<bool>> UsunDruzyneAdmin(int idDruzyny)
    {
        IDbContextTransaction? transakcja = context.Database.CurrentTransaction;
        var createdTransaction = false;
        if (transakcja == null)
        {
            transakcja = await context.Database.BeginTransactionAsync();
            createdTransaction = true;
        }
        try
        {
            if (idDruzyny <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id drużyny: " + idDruzyny));

            var druzyna = await druzynyRepository.GetDruzyna(idDruzyny);
            
            var statystykiRes = await statystykiService.UsunWymaganeStatystykiDruzyny(idDruzyny);
            if (!statystykiRes.Succeeded)
            {
                await transakcja.RollbackAsync();
                return statystykiRes;
            }

            // usuwamy wszystkie miejsca w drużynie, żeby nie było miejsc w drużynie, która już nie istnieje
            await druzynyRepository.DeleteMiejscaWDruzynie(idDruzyny);
            
            // usuwamy wszystkie wiadomości na czacie drużyny, żeby nie było wiadomości w drużynie, która już nie istnieje
            var czatDruzynowyRes = await wiadomoscService.DeleteWiadomosciDruzyny(idDruzyny);
            if (!czatDruzynowyRes.Succeeded)
            {
                await transakcja.RollbackAsync();
                return czatDruzynowyRes;
            }


            // usuwamy wszystkie powiadomienia związane z drużyną, żeby nie było powiadomień o drużynie, która już nie istnieje
            // czyli to będą zaproszenia do tej drużyny i u kapitana powiadomienia że ktoś dołączył/opuścił, ktoś odrzucił/zaakceptował zaproszenie .
            var powiadomieniaRes = await powiadomienieService.UsunPowiadomieniaZwiazaneZDruzyna(idDruzyny);
            if (!powiadomieniaRes.Succeeded)
            {
                await transakcja.RollbackAsync();
                return powiadomieniaRes;
            }
            
            // wysyłamy powiadomienia do wszystkich członków drużyny, że drużyna została usunięta
            var czlonkowieDruzyny = await druzynyRepository.GetMiejscaWDruzynie(idDruzyny);
            foreach (var miejsce in czlonkowieDruzyny)
            {
                if (miejsce.UzytkownikId == null) continue; // jeżeli miejsce jest puste, to nie wysyłamy powiadomienia
                
                await powiadomienieService.WyslijPowiadomienieORozwiazaniuDruzyny(
                    miejsce.UzytkownikId ??
                    0, // już odfiltrowaliśmy miejsca bez UzytkownikId, więc możemy bezpiecznie użyć ?? 0
                    druzyna.Nazwa
                );
                // nie przerywamy pętli jeżeli wysyłanie powiadomienia się nie powiedzie - coś jest nie tak z miejscem, ale drużyna i tak została usunięta, więc nie ma co robić w tej sytuacji
            }
            
            // wysyłamy powiadomienie do kapitana drużyny, że drużyna została usunięta przez administratora
            await powiadomienieService.WyslijPowiadomienieOUsunieciuDruzynyPrzezAdmina(
                druzyna.KapitanId,
                druzyna.Nazwa
            );
            
            // usuwamy drużynę z bazy danych
            await druzynyRepository.UsunDruzyne(idDruzyny);

            if (createdTransaction)
            {
                await transakcja.CommitAsync();
            }

            return ServiceResult<bool>.Ok(true);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
        catch (Exception e)
        {
            if (createdTransaction)
            {
                await transakcja.RollbackAsync();
                Console.WriteLine("Wystąpił błąd podczas usuwania drużyny: " + e.Message);
                return ServiceResult<bool>.Fail(500, [new ErrorItem("Wystąpił błąd podczas usuwania drużyny: " + e.Message)]);
            }
            // jeśli nie utworzyliśmy transakcji tutaj, rzucamy wyjątek dalej aby zewnętrzna transakcja mogła go obsłużyć/rollbackować
            throw;
        }
    }
    
    // czyści dane związane z drużynami dla danego użytkownika.
    // Usuwa drużyny, których jest kapitanem i usuwa go z drużyn, których nie jest kapitanem. Wysyła powiadomienia o rozwiązaniu drużyny do byłych członków drużyn, których był kapitanem.
    // używane przy usuwaniu konta
    public async Task<ServiceResult<bool>> UsunWszystkieDruzynyDlaUzytkownika(int idUzytkownika)
    {
        if (idUzytkownika <= 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Podano nieprawidłowe id użytkownika: " + idUzytkownika));
        
        var druzynyUzytkownika = await druzynyRepository.GetDruzynyUzytkownika(idUzytkownika);
        foreach (var druzyna in druzynyUzytkownika)
        {
            if (druzyna.KapitanId == idUzytkownika)
            {
                var usunDruzyneRes = await UsunDruzyne(druzyna.Id, idUzytkownika);
                if (!usunDruzyneRes.Succeeded) return ServiceResult<bool>.Fail(500, [new ErrorItem("Nie udało się usunąć drużyny o id " + druzyna.Id)]);
            }
            else
            {
                var opuscDruzyneRes = await druzynyService.OpuscDruzyne(druzyna.Id, idUzytkownika, true);
                if (!opuscDruzyneRes.Succeeded) return ServiceResult<bool>.Fail(500, [new ErrorItem("Nie udało się opuścić drużyny o id " + druzyna.Id)]);
            }
        }
        
        return  ServiceResult<bool>.NoContent(true);
    }
}