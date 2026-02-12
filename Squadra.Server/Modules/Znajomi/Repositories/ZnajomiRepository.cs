using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.DTO.Profil;
using Squadra.Server.Exceptions;
using Squadra.Server.Models;
using Squadra.Server.Modules.Znajomi.DTO;
using Squadra.Server.Services;

namespace Squadra.Server.Repositories;

public class ZnajomiRepository(
    AppDbContext context, 
    IProfilRepository profilRepository,
    IWiadomoscRepository wiadomoscRepository) : IZnajomiRepository
{
    
    public async Task<ICollection<Znajomi>> GetZnajomiUzytkownika(int id)
    {
        if (id < 1) throw new NieZnalezionoWBazieException("Użytkownik o id " + id + " nie istnieje");
        
        // sprawdzamy, czy użytkownik o podanym id istnieje, żeby w razie czego wywalić "Nie znaleziono w bazie exception"
        if(!context.Uzytkownik.Any(x => x.Id == id)) throw new NieZnalezionoWBazieException("Użytkownik o id " + id + " nie istnieje");
        
        return await context.Znajomi.Where(x => x.IdUzytkownika1 == id || x.IdUzytkownika2 == id).ToListAsync();
    }
    
    
    public async Task<DateTime?> GetDataOstatniegoOtwarciaCzatu(int idSprawdzajacego, int idZnajomego)
    {
        var znajomosc = await context.Znajomi.Where(x => x.IdUzytkownika1 == idSprawdzajacego && x.IdUzytkownika2 == idZnajomego || 
                                                          x.IdUzytkownika1 == idZnajomego && x.IdUzytkownika2 == idSprawdzajacego).FirstOrDefaultAsync();
        if(znajomosc == null) throw new NieZnalezionoWBazieException("Znajomosc o idUzytkownika1: " + idSprawdzajacego + " i idUzytkownika2: " + idZnajomego + " nie istnieje");
        
        return znajomosc.IdUzytkownika1 == idSprawdzajacego ? znajomosc.OstatnieOtwarcieCzatuUzytkownika1 : znajomosc.OstatnieOtwarcieCzatuUzytkownika2;
    }
    
    public async Task<bool> CreateZnajomosc(int idUzytkownika1, int idUzytkownika2)
    {
        // robimy to tylko po to, aby wywaliło "Nie znaleziono w bazie exception" w razie potrzeby
        var uzytkownik1 = await profilRepository.GetProfilUzytkownika(idUzytkownika1);
        var uzytkownik2 = await profilRepository.GetProfilUzytkownika(idUzytkownika2);
        
        var znajomosc = new Znajomi
        {
            IdUzytkownika1 = idUzytkownika1,
            IdUzytkownika2 = idUzytkownika2,
            DataNawiazaniaZnajomosci = DateOnly.FromDateTime(DateTime.Now)
        };
        await context.Znajomi.AddAsync(znajomosc);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteZnajomosc(int idUzytkownika1, int idUzytkownika2)
    {
        var znajomosc = await context.Znajomi.Where(x => x.IdUzytkownika1 == idUzytkownika1 && x.IdUzytkownika2 == idUzytkownika2).FirstOrDefaultAsync();
        if(znajomosc == null) throw new NieZnalezionoWBazieException("Znajomosc o idUzytkownika1: " + idUzytkownika1 + " i idUzytkownika2: " + idUzytkownika2 + " nie istnieje");
        // zaczynamy transakcję
        await using var transaction = await context.Database.BeginTransactionAsync();
        
        await wiadomoscRepository.DeleteWiadomosciUzytkownikow(idUzytkownika1, idUzytkownika2); // usuwamy ich wiadomości
        context.Znajomi.Remove(znajomosc); // usuwamy samą znajomość
        
        // kończymy transakcję
        await transaction.CommitAsync();
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteZnajomosciUzytkownika(int idUzytkownika)
    {
        var znajomosci = await context.Znajomi.Where(x => x.IdUzytkownika1 == idUzytkownika || x.IdUzytkownika2 == idUzytkownika).ToListAsync();
        foreach (var znajomosc in znajomosci) await DeleteZnajomosc(idUzytkownika, znajomosc.IdUzytkownika2);
        await context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> CzyJestZnajomosc(int idUzytkownika1, int idUzytkownika2)
    {
        return await context.Znajomi.AnyAsync(x => x.IdUzytkownika1 == idUzytkownika1 && x.IdUzytkownika2 == idUzytkownika2 || 
                                                   x.IdUzytkownika1 == idUzytkownika2 && x.IdUzytkownika2 == idUzytkownika1);
    }
    
    public async Task<bool> ZaktualizujOstatnieOtwarcieCzatu(int idOtwierajacego, int idZnajomego)
    {
        var znajomosc = await context.Znajomi.Where(x => x.IdUzytkownika1 == idOtwierajacego && x.IdUzytkownika2 == idZnajomego || 
                                                          x.IdUzytkownika1 == idZnajomego && x.IdUzytkownika2 == idOtwierajacego).FirstOrDefaultAsync();
        if(znajomosc == null) throw new NieZnalezionoWBazieException("Znajomosc o idUzytkownika1: " + idOtwierajacego + " i idUzytkownika2: " + idZnajomego + " nie istnieje");
        
        // aktualizujemy datę ostatniego otwarcia czatu dla użytkownika, który otwiera czat
        // mamy dwie zmienne w bazie danych, OstatnieOtwarcieCzatuUzytkownika1 i OstatnieOtwarcieCzatuUzytkownika2, więc musimy sprawdzić, który z nich aktualizować
        if (znajomosc.IdUzytkownika1 == idOtwierajacego) znajomosc.OstatnieOtwarcieCzatuUzytkownika1 = DateTime.Now;
        else znajomosc.OstatnieOtwarcieCzatuUzytkownika2 = DateTime.Now;
        
        context.Znajomi.Update(znajomosc);
        return await context.SaveChangesAsync() > 0;
    }
}