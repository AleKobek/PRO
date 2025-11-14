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
        
        if (uzytkownik == null) throw new NieZnalezionoWBazieException("Uzytkownik o id " + id + " nie istnieje");
        
        var role = (await userManager.GetRolesAsync(uzytkownik)).ToArray();

        
        return new UzytkownikResDto(uzytkownik.Id, uzytkownik.UserName ?? string.Empty, uzytkownik.Email ?? string.Empty, uzytkownik.PhoneNumber, uzytkownik.DataUrodzenia, role);
    }

    public async Task<DateTime?> GetOstatniaAktywnoscUzytkownika(int id)
    {
        var uzytkownik = await appDbContext.Uzytkownik.FindAsync(id);
        
        if (uzytkownik == null) throw new NieZnalezionoWBazieException("Uzytkownik o id " + id + " nie istnieje");

        return uzytkownik.OstatniaAktywnosc;
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
    public async Task<bool> UpdateUzytkownik(int id, UzytkownikUpdateDto uzytkownik)
    {
        
        var uzytkownikDoZmiany = await appDbContext.Uzytkownik.FindAsync(id);
        if(uzytkownikDoZmiany == null) throw new NieZnalezionoWBazieException("Uzytkownik o id " + id + " nie istnieje");
        
        uzytkownikDoZmiany.UserName = uzytkownik.Login;
        uzytkownikDoZmiany.NormalizedUserName = uzytkownik.Login.ToUpper();
        uzytkownikDoZmiany.Email = uzytkownik.Email;
        uzytkownikDoZmiany.NormalizedEmail = uzytkownik.Email.ToUpper();
        uzytkownikDoZmiany.PhoneNumber = uzytkownik.NumerTelefonu;
        uzytkownikDoZmiany.DataUrodzenia = uzytkownik.DataUrodzenia;
        
        await appDbContext.SaveChangesAsync();
        return true;
    }

    // lista stringów jest potrzebna do błędów w zmianie hasła, jak jest git zwracamy pustą
    public async Task<List<string>> UpdateHaslo(int idUzytkownika, string stareHaslo, string noweHaslo)
    {
        var bledy = new List<string>();
        // zakładamy, że service już sprawdziło, czy to właściwy użytkownik
        var user = await appDbContext.Uzytkownik.FindAsync(idUzytkownika);
        if(user == null) throw new NieZnalezionoWBazieException("Uzytkownik o id " + idUzytkownika + " nie istnieje");
        
        // musimy sprawdzić, czy hasła są takie same. Nie chodzi o argument, bo ktoś mógł sobie zmienić stare hasło
        var passwordHasher = new PasswordHasher<Uzytkownik>();
        // tu chyba możemy zignorować nulla, bo u nas każdy będzie to miał
        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, noweHaslo);
        // jeżeli hasła stare i nowe są takie same
        if (verificationResult == PasswordVerificationResult.Success)
        {
            bledy.Add("Nowe hasło jest takie samo, jak stare hasło");
            return bledy;
        }
        
        // nie są takie same, zmieniamy
        
        var result = await userManager.ChangePasswordAsync(user, stareHaslo, noweHaslo);
        if (!result.Succeeded)
        {
            bledy.AddRange(result.Errors.Select(e => e.Description));
            // będziemy to zwracać poniżej
        }
        // albo puste, albo z błędami z wyżej
        return bledy;
    }
    
    public async Task DeleteUzytkownik(int id)
    {
        var uzytkownik = await appDbContext.Uzytkownik.FindAsync(id);
        if(uzytkownik == null) throw new NieZnalezionoWBazieException("Uzytkownik o id " + id + " nie istnieje");
        // zaczynamy transakcję
        await using var transaction = await appDbContext.Database.BeginTransactionAsync();
        await profilRepository.DeleteProfil(id);
        await userManager.DeleteAsync(uzytkownik);
        // kończymy transakcję
        await transaction.CommitAsync();
        await appDbContext.SaveChangesAsync();
    }
    
    public async Task<bool> CzyLoginIstnieje(int id, string login)
    {
        var znormalizowanyLogin = login.Trim().ToUpper();
        var uzytkownik = await appDbContext.Uzytkownik.FindAsync(id);
        // jeżeli to ten sam login, co on ma, to na pewno jest unikalny
        // nie może być nullem, bo w kontrolerze przy sprawdzaniu czy edytuje swoje konto od razu go odrzuci
        if(uzytkownik.NormalizedUserName == znormalizowanyLogin) return false;
        return await appDbContext.Uzytkownik.AnyAsync(u => u.NormalizedUserName == znormalizowanyLogin);
    }

    public async Task<bool> CzyEmailIstnieje(int id, string email)
    {
        var znormalizowanyEmail = email.Trim().ToUpper();
        // sprawdzamy tylko, jeżeli to nie jest nowy użytkownik (a wtedy przekazujemy -1 jako id)
        if (id != -1){
            var uzytkownik = await appDbContext.Uzytkownik.FindAsync(id);
            // jeżeli to ten sam email, co on ma, to na pewno jest unikalny
            // nie może być nullem, bo w kontrolerze przy sprawdzaniu czy edytuje swoje konto od razu go odrzuci
            if (uzytkownik.NormalizedEmail == znormalizowanyEmail) return false;
        }
        return await appDbContext.Uzytkownik.AnyAsync(u => u.NormalizedEmail == znormalizowanyEmail);
    }
}