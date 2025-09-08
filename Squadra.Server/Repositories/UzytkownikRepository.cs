using Microsoft.EntityFrameworkCore;

namespace Squadra;


//TODO sprawdzić, czy w interfejsie się zgadzają funkcje
public class UzytkownikRepository(
    AppDbContext appDbContext)
    : IUzytkownikRepository
{
    public async Task<ICollection<UzytkownikDto>> GetUzytkownicy()
    {
        ICollection<UzytkownikDto> uzytkownicyDoZwrocenia = new List<UzytkownikDto>();
        ICollection<Uzytkownik> uzytkownicy = await appDbContext.Uzytkownik.ToListAsync();
        foreach (var uzytkownik in uzytkownicy)
        {
            uzytkownicyDoZwrocenia.Add(new UzytkownikDto(
                uzytkownik.Id, 
                uzytkownik.Login, 
                uzytkownik.Haslo,
                uzytkownik.NumerTelefonu,
                uzytkownik.DataUrodzenia,
                uzytkownik.StatusId
            ));
        }
        return uzytkownicyDoZwrocenia;
    }

    public async Task<UzytkownikDto?> GetUzytkownik(int id)
    {
        var uzytkownik = await appDbContext.Uzytkownik.FindAsync(id);
        
        if (uzytkownik == null) return null;
        
        return new UzytkownikDto(uzytkownik.Id, uzytkownik.Login, uzytkownik.Haslo, uzytkownik.NumerTelefonu, uzytkownik.DataUrodzenia, uzytkownik.StatusId);
    }

    // zmienić na użytkownik create dto w add i update, może scalić?
    // to jednak put czy bez put? w przykładach mam z put, może jednak nie scalać
    public async Task<Uzytkownik?> AddUzytkownik(Uzytkownik uzytkownik)
    {
        appDbContext.Uzytkownik.Add(uzytkownik);
        await appDbContext.SaveChangesAsync();
        return uzytkownik;
    }

    public async Task<Uzytkownik?> UpdateUzytkownik(Uzytkownik uzytkownik)
    {
        var uzytkownikDoEdycji = await appDbContext.Uzytkownik.FindAsync(uzytkownik.Id);
        if(uzytkownikDoEdycji == null) return null;
        
        appDbContext.Entry(uzytkownikDoEdycji).CurrentValues.SetValues(uzytkownik);
        await appDbContext.SaveChangesAsync();
        return uzytkownik;
        
    }
    
    
}