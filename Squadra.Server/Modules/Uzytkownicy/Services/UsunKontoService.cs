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
    IZnajomiService znajomiService,
    IDruzynyService druzynyService,
    AppDbContext context) : IUsunKontoService
{
    public async Task<ServiceResult<bool>> UsunKonto(int id)
    {
        var transakcja = await context.Database.BeginTransactionAsync();
        
        try
        {
            var znajomosciRes = await znajomiService.DeleteZnajomosciUzytkownika(id);
            if (!znajomosciRes.Succeeded) 
            {
                await transakcja.RollbackAsync();
                return znajomosciRes;
            }
            
            var powiadomieniaRes = await powiadomienieService.DeletePowiadomieniaUzytkownika(id);
            if (!powiadomieniaRes.Succeeded) 
            {
                await transakcja.RollbackAsync();
                return powiadomieniaRes;
            }
            
            var integracjeRes = await integracjeZewnetrzneService.PrzerwijIntegracjeUzytkownika(id);
            if(!integracjeRes.Succeeded) 
            {
                await transakcja.RollbackAsync();
                return integracjeRes;
            }
            
            // wyrzucamy go ze wszystkich drużyn
            var wyrzucanieRes = await druzynyService.WyrzucUzytkownikaZeWszystkichDruzyn(id);
            if(!wyrzucanieRes.Succeeded)
            {
                await transakcja.RollbackAsync();
                return wyrzucanieRes;
            }
            
            // usuwamy wszystkie drużyny, których jest kapitanem
            var usuwanieRes = await druzynyService.UsunWszystkieDruzynyUzytkownika(id);
            if(!usuwanieRes.Succeeded)
            {
                await transakcja.RollbackAsync();
                return usuwanieRes;
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