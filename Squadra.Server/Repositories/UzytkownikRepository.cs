using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.DTO.Profil;
using Squadra.Server.DTO.Uzytkownik;
using Squadra.Server.Models;

namespace Squadra.Server.Repositories;



public class UzytkownikRepository(
    AppDbContext appDbContext,
    IProfilRepository profilRepository)
    : IUzytkownikRepository
{
    
    public async Task<ICollection<UzytkownikResDto>> GetUzytkownicy()
    {
        ICollection<UzytkownikResDto> uzytkownicyDoZwrocenia = new List<UzytkownikResDto>();
        ICollection<Uzytkownik> uzytkownicy = await appDbContext.Uzytkownik.ToListAsync();
        
        foreach (var uzytkownik in uzytkownicy)
        {
            uzytkownicyDoZwrocenia.Add(new UzytkownikResDto(
                uzytkownik.Id, 
                uzytkownik.Login, 
                uzytkownik.Haslo,
                uzytkownik.Email,
                uzytkownik.NumerTelefonu,
                uzytkownik.DataUrodzenia
            ));
        }
        return uzytkownicyDoZwrocenia;
    }

    public async Task<UzytkownikResDto> GetUzytkownik(int id)
    {
        var uzytkownik = await appDbContext.Uzytkownik.FindAsync(id);
        
        if (uzytkownik == null) throw new Exception("Uzytkownik o id " + id + " nie istnieje");
        
        
        return new UzytkownikResDto(uzytkownik.Id, uzytkownik.Login, uzytkownik.Haslo, uzytkownik.Email, uzytkownik.NumerTelefonu, uzytkownik.DataUrodzenia);
    }

    public async Task<UzytkownikResDto> CreateUzytkownik(UzytkownikCreateDto uzytkownik)
    {
        var id = await GetNastepneId();
        var uzytkownikDoDodania = new Uzytkownik {
            Id = id,
            Login = uzytkownik.Login,
            Haslo = uzytkownik.HasloHashed,
            Email = uzytkownik.Email,
            NumerTelefonu = uzytkownik.NumerTelefonu,
            DataUrodzenia = uzytkownik.DataUrodzenia,
        };
        // zaczynamy transakcję
        await using var transaction = await appDbContext.Database.BeginTransactionAsync();
        await appDbContext.Uzytkownik.AddAsync(uzytkownikDoDodania);
        await profilRepository.CreateProfil(new ProfilCreateReqDto(id, uzytkownik.Pseudonim));
        await appDbContext.SaveChangesAsync();
        // kończymy transakcję
        await transaction.CommitAsync();
        return new UzytkownikResDto(
            id,
            uzytkownik.Login,
            uzytkownik.HasloHashed,
            uzytkownik.Email,
            uzytkownik.NumerTelefonu,
            uzytkownik.DataUrodzenia
        );
    }

    public async Task<UzytkownikResDto> UpdateUzytkownik(UzytkownikUpdateDto uzytkownik)
    {
        
        
        var uzytkownikDoZmiany = await appDbContext.Uzytkownik.FindAsync(uzytkownik.Id);
        if(uzytkownikDoZmiany == null) throw new Exception("Uzytkownik o id " + uzytkownik.Id + " nie istnieje");
        
        uzytkownikDoZmiany.Login = uzytkownik.Login;
        uzytkownikDoZmiany.Haslo = uzytkownik.Haslo;
        uzytkownikDoZmiany.Email = uzytkownik.Email;
        uzytkownikDoZmiany.NumerTelefonu = uzytkownik.NumerTelefonu;
        uzytkownikDoZmiany.DataUrodzenia = uzytkownik.DataUrodzenia;
        
        await appDbContext.SaveChangesAsync();
        
        return await GetUzytkownik(uzytkownik.Id);
        
    }
    
    public async Task DeleteUzytkownik(int id)
    {
        var uzytkownik = await appDbContext.Uzytkownik.FindAsync(id);
        if(uzytkownik == null) throw new Exception("Uzytkownik o id " + id + " nie istnieje");
        await profilRepository.DeleteProfil(id);
        appDbContext.Uzytkownik.Remove(uzytkownik);
        await appDbContext.SaveChangesAsync();
    }

    private async Task<int> GetNastepneId()
    {
        var najwyzszeId = await appDbContext.Uzytkownik.MaxAsync(x => x.Id);
        return najwyzszeId + 1;
    }
    
    // na przyszłość do rejestracji
    public async Task<bool> CzyLoginIstnieje(string login)
    {
        var q = login.Trim();
        return await appDbContext.Uzytkownik.AnyAsync(u => u.Login == q);
    }

    public async Task<bool> CzyEmailIstnieje(string email)
    {
        var q = email.Trim().ToLower();
        return await appDbContext.Uzytkownik.AnyAsync(u => u.Email == q);
    }
}