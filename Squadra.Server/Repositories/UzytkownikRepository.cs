using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Squadra.Server.Context;
using Squadra.Server.DTO.Profil;
using Squadra.Server.DTO.Uzytkownik;
using Squadra.Server.Exceptions;
using Squadra.Server.Models;

namespace Squadra.Server.Repositories;



public class UzytkownikRepository(
    AppDbContext appDbContext,
    IProfilRepository profilRepository,
    UserManager<Uzytkownik> userManager,
    RoleManager<IdentityRole<int>> roleManager)
    : IUzytkownikRepository
{
    
    public async Task<ICollection<UzytkownikResDto>> GetUzytkownicy()
    {
        ICollection<UzytkownikResDto> uzytkownicyDoZwrocenia = new List<UzytkownikResDto>();
        ICollection<Uzytkownik> uzytkownicy = await appDbContext.Uzytkownik.ToListAsync();
        
        foreach (var uzytkownik in uzytkownicy)
        {
            var role = (await userManager.GetRolesAsync(uzytkownik)).ToArray();

            uzytkownicyDoZwrocenia.Add(new UzytkownikResDto(
                uzytkownik.Id, 
                uzytkownik.UserName ?? string.Empty, 
                uzytkownik.Email ?? string.Empty,
                uzytkownik.PhoneNumber,
                uzytkownik.DataUrodzenia,
                role
            ));
        }
        return uzytkownicyDoZwrocenia;
    }

    public async Task<UzytkownikResDto> GetUzytkownik(int id)
    {
        var uzytkownik = await appDbContext.Uzytkownik.FindAsync(id);
        
        if (uzytkownik == null) throw new Exception("Uzytkownik o id " + id + " nie istnieje");
        
        var role = (await userManager.GetRolesAsync(uzytkownik)).ToArray();

        
        return new UzytkownikResDto(uzytkownik.Id, uzytkownik.UserName ?? string.Empty, uzytkownik.Email ?? string.Empty, uzytkownik.PhoneNumber, uzytkownik.DataUrodzenia, role);
    }

    public async Task<UzytkownikResDto> CreateUzytkownik(UzytkownikCreateDto uzytkownik)
    {
        var uzytkownikDoDodania = new Uzytkownik {
            UserName = uzytkownik.Login,
            NormalizedUserName = uzytkownik.Login.ToUpper(),
            Email = uzytkownik.Email,
            NormalizedEmail = uzytkownik.Email.ToUpper(),
            PhoneNumber = uzytkownik.NumerTelefonu,
            DataUrodzenia = uzytkownik.DataUrodzenia,
        };
        // zaczynamy transakcję
        await using var transaction = await appDbContext.Database.BeginTransactionAsync();
        // Identity zadba o hash hasła :)
        var result = await userManager.CreateAsync(uzytkownikDoDodania, uzytkownik.Haslo);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => $"{e.Code}:{e.Description}"));
            throw new InvalidOperationException($"Nie udało się utworzyć użytkownika: {errors}");
        }

        // samo tworzy nam id!
        var id = uzytkownikDoDodania.Id;
        await profilRepository.CreateProfil(new ProfilCreateReqDto(id, uzytkownik.Pseudonim));
        await appDbContext.SaveChangesAsync();
        // dodajemy użytkownika do roli
        if (await roleManager.RoleExistsAsync("Uzytkownik"))
            await userManager.AddToRoleAsync(uzytkownikDoDodania, "Uzytkownik");

        var role = (await userManager.GetRolesAsync(uzytkownikDoDodania)).ToArray();

        // kończymy transakcję
        await transaction.CommitAsync();
        return new UzytkownikResDto(
            id,
            uzytkownik.Login,
            uzytkownik.Email,
            uzytkownik.NumerTelefonu,
            uzytkownik.DataUrodzenia,
            role
        );
    }

    // do zmiany hasła będzie oddzielne
    public async Task<UzytkownikResDto> UpdateUzytkownik(UzytkownikUpdateDto uzytkownik)
    {
        
        var uzytkownikDoZmiany = await appDbContext.Uzytkownik.FindAsync(uzytkownik.Id);
        if(uzytkownikDoZmiany == null) throw new NieZnalezionoWBazieException("Uzytkownik o id " + uzytkownik.Id + " nie istnieje");
        
        uzytkownikDoZmiany.UserName = uzytkownik.Login;
        uzytkownikDoZmiany.NormalizedUserName = uzytkownik.Login.ToUpper();
        uzytkownikDoZmiany.Email = uzytkownik.Email;
        uzytkownikDoZmiany.NormalizedEmail = uzytkownik.Email.ToUpper();
        uzytkownikDoZmiany.PhoneNumber = uzytkownik.NumerTelefonu;
        uzytkownikDoZmiany.DataUrodzenia = uzytkownik.DataUrodzenia;
        
        await appDbContext.SaveChangesAsync();
        
        var role = (await userManager.GetRolesAsync(uzytkownikDoZmiany)).ToArray();

        
        return new UzytkownikResDto(
            uzytkownik.Id,
            uzytkownik.Login,
            uzytkownik.Email,
            uzytkownik.NumerTelefonu,
            uzytkownik.DataUrodzenia,
            role
        );
        
    }
    
    public async Task DeleteUzytkownik(int id)
    {
        var uzytkownik = await appDbContext.Uzytkownik.FindAsync(id);
        if(uzytkownik == null) throw new Exception("Uzytkownik o id " + id + " nie istnieje");
        // zaczynamy transakcję
        await using var transaction = await appDbContext.Database.BeginTransactionAsync();
        await profilRepository.DeleteProfil(id);
        await userManager.DeleteAsync(uzytkownik);
        // kończymy transakcję
        await transaction.CommitAsync();
        await appDbContext.SaveChangesAsync();
    }
    
    public async Task<bool> CzyLoginIstnieje(string login)
    {
        var q = login.Trim().ToUpper();
        return await appDbContext.Uzytkownik.AnyAsync(u => u.UserName == q);
    }

    public async Task<bool> CzyEmailIstnieje(string email)
    {
        var q = email.Trim().ToUpper();
        return await appDbContext.Uzytkownik.AnyAsync(u => u.Email == q);
    }
}