using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.DTO.Profil;
using Squadra.Server.Exceptions;
using Squadra.Server.Models;

namespace Squadra.Server.Repositories;

public class ZnajomiRepository(
    AppDbContext context, 
    IProfilRepository profilRepository,
    IWiadomoscRepository wiadomoscRepository) : IZnajomiRepository
{

    public async Task<ICollection<ProfilGetResDto>> GetZnajomiUzytkownika(int id)
    {
        // najpierw z bazy znajomych pobieramy id wszystkich osób, które są w parze z naszym
        ICollection<Znajomi> znajomi = await context.Znajomi.Where(x => x.IdUzytkownika1 == id || x.IdUzytkownika2 == id).ToListAsync();
        ICollection<ProfilGetResDto> listaDoZwrocenia = new List<ProfilGetResDto>();
        // dla każdego użytkownika bierzemy jego profil
        foreach (var uzytkownik in znajomi)
        {
            var idZnajomego = uzytkownik.IdUzytkownika1 == id ? uzytkownik.IdUzytkownika2 : uzytkownik.IdUzytkownika1;
            listaDoZwrocenia.Add(await profilRepository.GetProfilUzytkownika(idZnajomego));
        }
        return listaDoZwrocenia;
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
        return await context.Znajomi.AnyAsync(x => x.IdUzytkownika1 == idUzytkownika1 && x.IdUzytkownika2 == idUzytkownika2);
    }
}