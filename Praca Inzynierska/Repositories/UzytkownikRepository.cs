using Microsoft.EntityFrameworkCore;
using Praca_Inzynierska.Context;
using Praca_Inzynierska.DTO;
using Praca_Inzynierska.Models;

namespace Praca_Inzynierska.Repositories;

public class UzytkownikRepository : IUzytkownikRepository
{
    private readonly AppDbContext _context;
    
    private readonly IJezykRepository _jezykRepository;
    private readonly IRegionRepository _regionRepository;
    
    
    public UzytkownikRepository(AppDbContext appDbContext)
    {
        _context = appDbContext;
    }


    public async Task<ICollection<UzytkownikOrazProfilDto>> GetUzytkownicy()
    {
        ICollection<UzytkownikOrazProfilDto> uzytkownicyDoZwrocenia = new List<UzytkownikOrazProfilDto>();
        ICollection<Uzytkownik> uzytkownicy = await _context.Uzytkownik.ToListAsync();
        ICollection<Profil> profile = await _context.Profil.ToListAsync();

        foreach (var uzytkownik in uzytkownicy)
        {
            Profil? profil = profile.FirstOrDefault(x => x.IdUzytkownika == uzytkownik.Id);
            var jezykiUzytkownika = await _jezykRepository.GetJezykiUzytkownika(uzytkownik.Id);
            RegionDto? region = await _regionRepository.GetRegion(uzytkownik.RegionId);
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
        var uzytkownik = await _context.Uzytkownik.FindAsync(id);
        
        if (uzytkownik == null) return null;
        
        var profil = await _context.Profil.Where(x => x.IdUzytkownika == id).FirstOrDefaultAsync();
        string? zaimki = null;
        string? opis = null;
        if (profil != null)
        {
            zaimki = profil.Zaimki;
            opis = profil.Opis;
        }
        
        var region = await _regionRepository.GetRegion(uzytkownik.RegionId);
        var jezykiUzytkownika = await _jezykRepository.GetJezykiUzytkownika(uzytkownik.Id);
       
        return new UzytkownikOrazProfilDto(uzytkownik.Id, uzytkownik.Login, uzytkownik.Pseudonim, uzytkownik.Haslo, region, uzytkownik.NumerTelefonu, zaimki, opis, jezykiUzytkownika);
    }
    
    // wszędzie CRUD i robić front
    
}