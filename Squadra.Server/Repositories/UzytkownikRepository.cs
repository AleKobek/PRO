using Microsoft.EntityFrameworkCore;

namespace Squadra;

public class UzytkownikRepository(
    AppDbContext appDbContext,
    IJezykRepository jezykRepository,
    IRegionRepository regionRepository)
    : IUzytkownikRepository
{
    public async Task<ICollection<UzytkownikOrazProfilDto>> GetUzytkownicy()
    {
        ICollection<UzytkownikOrazProfilDto> uzytkownicyDoZwrocenia = new List<UzytkownikOrazProfilDto>();
        ICollection<Uzytkownik> uzytkownicy = await appDbContext.Uzytkownik.ToListAsync();
        ICollection<Profil> profile = await appDbContext.Profil.ToListAsync();

        foreach (var uzytkownik in uzytkownicy)
        {
            Profil? profil = profile.FirstOrDefault(x => x.IdUzytkownika == uzytkownik.Id);
            var jezykiUzytkownika = await jezykRepository.GetJezykiUzytkownika(uzytkownik.Id);
            RegionDto? region = await regionRepository.GetRegion(uzytkownik.RegionId);
            if (profil != null)
            {
                uzytkownicyDoZwrocenia.Add(new UzytkownikOrazProfilDto(
                    uzytkownik.Id, 
                    uzytkownik.Login, 
                    uzytkownik.Pseudonim, 
                    uzytkownik.Haslo, 
                    region,
                    uzytkownik.NumerTelefonu,
                    profil.Zaimki,
                    profil.Opis,
                    jezykiUzytkownika
                    ));
            }
            else
            {
                uzytkownicyDoZwrocenia.Add(new UzytkownikOrazProfilDto(
                    uzytkownik.Id, 
                    uzytkownik.Login, 
                    uzytkownik.Pseudonim, 
                    uzytkownik.Haslo, 
                    region,
                    uzytkownik.NumerTelefonu,
                    null,
                    null,
                    jezykiUzytkownika
                ));
            }
            
        }
        return uzytkownicyDoZwrocenia;
    }

    public async Task<UzytkownikOrazProfilDto?> GetUzytkownik(int id)
    {
        var uzytkownik = await appDbContext.Uzytkownik.FindAsync(id);
        
        if (uzytkownik == null) return null;
        
        var profil = await appDbContext.Profil.Where(x => x.IdUzytkownika == id).FirstOrDefaultAsync();
        string? zaimki = null;
        string? opis = null;
        if (profil != null)
        {
            zaimki = profil.Zaimki;
            opis = profil.Opis;
        }
        
        var region = await regionRepository.GetRegion(uzytkownik.RegionId);
        var jezykiUzytkownika = await jezykRepository.GetJezykiUzytkownika(uzytkownik.Id);
       
        return new UzytkownikOrazProfilDto(uzytkownik.Id, uzytkownik.Login, uzytkownik.Pseudonim, uzytkownik.Haslo, region, uzytkownik.NumerTelefonu, zaimki, opis, jezykiUzytkownika);
    }

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
    
    
    // robić front
    
}