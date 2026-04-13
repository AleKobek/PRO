using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Platformy.DTO;
using Squadra.Server.Modules.Platformy.Models;

namespace Squadra.Server.Modules.Platformy.Repositories;

public class PlatformaRepository(AppDbContext context) : IPlatformaRepository{
    
    public async Task<ICollection<Platforma>> GetPlatformy()
    {
        return await context.Platforma.ToListAsync();
    }
    
    public async Task<Platforma> GetPlatforma(int id)
    {
        var platforma = await context.Platforma.FirstOrDefaultAsync(p => p.Id == id);
        if(platforma is null)
            throw new NieZnalezionoWBazieException("Nie znaleziono platformy o podanym id.");
        return platforma;
    }
    
    public async Task<ICollection<PlatformaUzytkownikaDTO>> GetPlatformyUzytkownika(int idUzytkownika)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik == null) throw new NieZnalezionoWBazieException("Uzytkownik o id " + idUzytkownika + " nie istnieje.");
        
        var platformyUzytkownika = await context.UzytkownikPlatforma.Where(up => up.UzytkownikId == idUzytkownika).ToListAsync();
        var platformy = new List<PlatformaUzytkownikaDTO>();
        foreach (var up in platformyUzytkownika)
        {
            try
            {
                var platforma = await GetPlatforma(up.PlatformaId);
                platformy.Add(new PlatformaUzytkownikaDTO(
                    up.PlatformaId,
                    platforma.Nazwa,
                    platforma.Logo,
                    up.PseudonimNaPlatformie
                ));
            }
            catch (NieZnalezionoWBazieException)
            {
                continue; // jeśli platforma nie istnieje, pomijamy ją
            }
        }
        return platformy;
    }
    
    // funkcja aktualizująca platformy użytkownika, czyli usuwająca wszystkie stare wpisy z tabeli UzytkownikPlatforma dla danego idUzytkownika i dodająca nowe wpisy
    public async Task<bool> UpdatePlatformyUzytkownika(int idUzytkownika, List <UzytkownikPlatforma> nowePlatformy)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik == null) throw new NieZnalezionoWBazieException("Uzytkownik o id " + idUzytkownika + " nie istnieje.");
        
            // tutaj się zaczyna, dostajemy listę
            
            await using var transaction = await context.Database.BeginTransactionAsync();
            
            // usuwamy wszystkie stare wpisy z tabeli UzytkownikPlatforma dla danego idUzytkownika
            var starePlatformyUzytkownika = await context.UzytkownikPlatforma.Where(up => up.UzytkownikId == idUzytkownika).ToListAsync();
            context.UzytkownikPlatforma.RemoveRange(starePlatformyUzytkownika);
            
            // dodajemy wszystkie platformy użytkownika do bazy danych
            context.UzytkownikPlatforma.AddRange(nowePlatformy);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return true;
    }
    
    // funkcja usuwająca wszystkie platformy użytkownika, czyli wszystkie wpisy z tabeli UzytkownikPlatforma dla danego idUzytkownika
    // zakładamy, że usunęliśmy już wszystkie gry na platformach użytkownika, więc nie musimy się martwić o blokady
    public async Task<bool> UsunPlatformyUzytkownika(int idUzytkownika)
    {
        var uzytkownik = await context.Uzytkownik.FindAsync(idUzytkownika);
        if (uzytkownik == null) throw new NieZnalezionoWBazieException("Uzytkownik o id " + idUzytkownika + " nie istnieje.");
        
        var platformyUzytkownika = await context.UzytkownikPlatforma.Where(up => up.UzytkownikId == idUzytkownika).ToListAsync();
        context.UzytkownikPlatforma.RemoveRange(platformyUzytkownika);
        await context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> CreatePlatforma(int id, string nazwa, byte[] logo)
    {
        var platforma = new Platforma
        {
            Id = id,
            Nazwa = nazwa,
            Logo = logo
        };
        context.Platforma.Add(platforma);
        await context.SaveChangesAsync();
        return true;
    }
}