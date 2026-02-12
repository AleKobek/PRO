using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.DTO.Wiadomosc;
using Squadra.Server.Exceptions;

namespace Squadra.Server.Repositories;

public class WiadomoscRepository(AppDbContext context) : IWiadomoscRepository
{
    
    public async Task<WiadomoscDto> GetWiadomosc(int id)
    {
        var wiadomosc = await context.Wiadomosc.FindAsync(id);
        if(wiadomosc == null) throw new NieZnalezionoWBazieException("Wiadomosc o id " + id + " nie istnieje");
        return new WiadomoscDto(
            wiadomosc.IdNadawcy,
            wiadomosc.IdOdbiorcy,
            wiadomosc.DataWyslania,
            wiadomosc.Tresc,
            wiadomosc.IdTypuWiadomosci
        );
    }

    public async Task<ICollection<WiadomoscDto>> GetWiadomosci(int idUzytkownika1, int idUzytkownika2)
    {
        var wiadomosci = await context.Wiadomosc
            .Where(x => (x.IdNadawcy == idUzytkownika1 && x.IdOdbiorcy == idUzytkownika2) ||
                                  (x.IdNadawcy == idUzytkownika2 && x.IdOdbiorcy == idUzytkownika1))
            .ToListAsync();
        wiadomosci.Sort((x,y) => x.DataWyslania.CompareTo(y.DataWyslania));
        return wiadomosci.Select(wiadomosc => new WiadomoscDto(
            wiadomosc.IdNadawcy,
            wiadomosc.IdOdbiorcy,
            wiadomosc.DataWyslania,
            wiadomosc.Tresc,
            wiadomosc.IdTypuWiadomosci
        )).ToList();
    }
    
    // nie obchodzi nas, kto jest nadawcą, a kto odbiorcą, więc bierzemy max z obu
    public async Task<DateTime?> GetDataNajnowszejWiadomosci(int idUzytkownika1, int idUzytkownika2) {
        var wiadomosci = await context.Wiadomosc
            .Where(x => (x.IdNadawcy == idUzytkownika1 && x.IdOdbiorcy == idUzytkownika2) ||
                                  (x.IdNadawcy == idUzytkownika2 && x.IdOdbiorcy == idUzytkownika1))
            .ToListAsync();
        if(wiadomosci.Count == 0) return null;
        return wiadomosci.Max(x => x.DataWyslania);
    }
    
    public async Task<bool> CreateWiadomosc(WiadomoscCreateDto wiadomosc, int idNadawcy)
    {
        var wiadomoscDoDodania = new Models.Wiadomosc
        {
            IdNadawcy = idNadawcy,
            IdOdbiorcy = wiadomosc.IdOdbiorcy,
            DataWyslania = DateTime.Now, // liczymy moment, w którym dotrze do bazy
            Tresc = wiadomosc.Tresc,
            IdTypuWiadomosci = wiadomosc.IdTypuWiadomosci
        };
        await context.Wiadomosc.AddAsync(wiadomoscDoDodania);
        return await context.SaveChangesAsync() > 0; // zwracamy true, jeżeli dodano więcej niż 0 rekordów, czyli się udało
    }

    // przy usuwaniu znajomości usuwamy też wszystkie wiadomości między tymi użytkownikami
    public async Task<bool> DeleteWiadomosciUzytkownikow(int idUzytkownika1, int idUzytkownika2)
    {
        var wiadomosci = await context.Wiadomosc
            .Where(x => (x.IdNadawcy == idUzytkownika1 && x.IdOdbiorcy == idUzytkownika2) ||
                                  (x.IdNadawcy == idUzytkownika2 && x.IdOdbiorcy == idUzytkownika1))
            .ToListAsync();
        context.Wiadomosc.RemoveRange(wiadomosci);
        await context.SaveChangesAsync();
        return true;
    }
}