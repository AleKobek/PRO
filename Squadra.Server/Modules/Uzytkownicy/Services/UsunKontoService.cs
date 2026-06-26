using Squadra.Server.Context;
using Squadra.Server.Modules.Drużyny.Services;
using Squadra.Server.Modules.IntegracjeZewnetrzne.Services;
using Squadra.Server.Modules.Powiadomienia.Services;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Znajomosci.Services;

namespace Squadra.Server.Modules.Uzytkownicy.Services;

public class UsunKontoService (
    IUzytkownikService uzytkownikService,
    IIntegracjeZewnetrzneService integracjeZewnetrzneService,
    IPowiadomienieService powiadomienieService,
    IDeleteDruzynaService deleteDruzynaService,
    IDeleteZnajomoscService deleteZnajomoscService,
    AppDbContext context) : IUsunKontoService
{
    public async Task<ServiceResult<bool>> UsunKonto(int id)
    {
        var transakcja = await context.Database.BeginTransactionAsync();
        
        try
        {
            var znajomosciRes = await deleteZnajomoscService.DeleteZnajomosciUzytkownika(id);
            if (!znajomosciRes.Succeeded) 
            {
                await transakcja.RollbackAsync();
                return znajomosciRes;
            }
            
            var integracjeRes = await integracjeZewnetrzneService.PrzerwijIntegracjeUzytkownika(id, true);
            if(!integracjeRes.Succeeded) 
            {
                await transakcja.RollbackAsync();
                return integracjeRes;
            }
            
            // wyrzucamy go ze wszystkich drużyn
            var wyrzucanieRes = await deleteDruzynaService.UsunWszystkieDruzynyDlaUzytkownika(id);
            if(!wyrzucanieRes.Succeeded)
            {
                await transakcja.RollbackAsync();
                return wyrzucanieRes;
            }
            
            var powiadomieniaRes = await powiadomienieService.DeletePowiadomieniaZwiazaneZUzytkownikiem(id);
            if (!powiadomieniaRes.Succeeded) 
            {
                await transakcja.RollbackAsync();
                return powiadomieniaRes;
            }
            
            var uzytkownikRes = await uzytkownikService.DeleteUzytkownik(id);
            if(!uzytkownikRes.Succeeded) 
            {
                await transakcja.RollbackAsync();
                return uzytkownikRes;
            }
            
            await transakcja.CommitAsync();
            return ServiceResult<bool>.NoContent(true);
        }
        catch
        {
            await transakcja.RollbackAsync();
            throw;
        }
        finally
        {
            await transakcja.DisposeAsync();
        }
    }
}