using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Wiadomosci.Services;
using Squadra.Server.Modules.Znajomosci.Repositories;

namespace Squadra.Server.Modules.Znajomosci.Services;

public class DeleteZnajomoscService(
    AppDbContext context,
    IZnajomiRepository znajomiRepository,
    IWiadomoscService wiadomoscService) : IDeleteZnajomoscService
{
    public async Task<ServiceResult<bool>> DeleteZnajomosc(int idUzytkownikaInicjujacego, int idUzytkownika2)
    {
        // Jeśli transakcja już istnieje (np. z UsunKonto), nie otwieramy nowej
        var czyToNowaTransakcja = context.Database.CurrentTransaction == null;
        var transakcja = czyToNowaTransakcja 
            ? await context.Database.BeginTransactionAsync() 
            : null;
        try
        {
            if (idUzytkownikaInicjujacego < 1)
                return ServiceResult<bool>.BadRequest(
                    new ErrorItem("Nieprawidłowe id użytkownika inicjującego: " + idUzytkownikaInicjujacego));
            if (idUzytkownika2 < 1)
                return ServiceResult<bool>.BadRequest(
                    new ErrorItem("Nieprawidłowe id usuwanego znajomego: " + idUzytkownika2));
            
            await znajomiRepository.GetZnajomosc(idUzytkownikaInicjujacego, idUzytkownika2); // tylko po to, żeby wywaliło "Nie znaleziono w bazie exception" w razie potrzeby
            
            var wiadomosciRes = await wiadomoscService.DeleteWiadomosciPrywatneUzytkownikow(idUzytkownikaInicjujacego, idUzytkownika2); // usuwamy ich wiadomości
            if (!wiadomosciRes.Succeeded)
            {
                if(czyToNowaTransakcja) await transakcja!.RollbackAsync();
                return wiadomosciRes;
            }

            await znajomiRepository.DeleteZnajomosc(idUzytkownikaInicjujacego, idUzytkownika2);
            if (czyToNowaTransakcja) await transakcja!.CommitAsync();
            return ServiceResult<bool>.NoContent(true);
        }
        catch (NieZnalezionoWBazieException e)
        {
            if(czyToNowaTransakcja) await transakcja!.RollbackAsync();
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> DeleteZnajomosciUzytkownika(int idUzytkownika)
    {
        var znajomosci = await context.Znajomi.Where(x => x.IdUzytkownika1 == idUzytkownika || x.IdUzytkownika2 == idUzytkownika).ToListAsync();
        foreach (var znajomosc in znajomosci)
        {
            if(idUzytkownika == znajomosc.IdUzytkownika1) await DeleteZnajomosc(idUzytkownika, znajomosc.IdUzytkownika2);
            else await DeleteZnajomosc(idUzytkownika, znajomosc.IdUzytkownika1);
        }
        await context.SaveChangesAsync();
        return ServiceResult<bool>.NoContent(true);
    }
}