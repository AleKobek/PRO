using Microsoft.EntityFrameworkCore;

namespace Squadra;



public class UzytkownikRepository(
    AppDbContext appDbContext,
    IProfilRepository profilRepository,
    IStatusRepository statusRepository)
    : IUzytkownikRepository
{
    
    public async Task<ICollection<UzytkownikResDto>> GetUzytkownicy()
    {
        ICollection<UzytkownikResDto> uzytkownicyDoZwrocenia = new List<UzytkownikResDto>();
        ICollection<Uzytkownik> uzytkownicy = await appDbContext.Uzytkownik.ToListAsync();
        
        foreach (var uzytkownik in uzytkownicy)
        {
            // jak ktoś akurat zły status, ustawiamy domyślny, czyli offline. nie ma sensu rzucać błędu tylko po to
            var status = await statusRepository.GetStatus(uzytkownik.StatusId) ?? statusRepository.GetStatusDomyslny();
            
            uzytkownicyDoZwrocenia.Add(new UzytkownikResDto(
                uzytkownik.Id, 
                uzytkownik.Login, 
                uzytkownik.Haslo,
                uzytkownik.Email,
                uzytkownik.NumerTelefonu,
                uzytkownik.DataUrodzenia,
                status
            ));
        }
        return uzytkownicyDoZwrocenia;
    }

    public async Task<UzytkownikResDto> GetUzytkownik(int id)
    {
        var uzytkownik = await appDbContext.Uzytkownik.FindAsync(id);
        
        if (uzytkownik == null) throw new Exception("Uzytkownik o id " + id + " nie istnieje");
        
        var status = await statusRepository.GetStatus(uzytkownik.StatusId) ?? statusRepository.GetStatusDomyslny();

        
        return new UzytkownikResDto(uzytkownik.Id, uzytkownik.Login, uzytkownik.Haslo, uzytkownik.Email, uzytkownik.NumerTelefonu, uzytkownik.DataUrodzenia, status);
    }

    public async Task<UzytkownikResDto> CreateUzytkownik(UzytkownikCreateDto uzytkownik)
    {
        var uzytkownikDoDodania = new Uzytkownik { 
            Id = uzytkownik.Id,
            Login = uzytkownik.Login,
            Haslo = uzytkownik.Haslo,
            Email = uzytkownik.Email,
            NumerTelefonu = uzytkownik.NumerTelefonu,
            DataUrodzenia = uzytkownik.DataUrodzenia,
        };
        // zaczynamy transakcję
        await using var transaction = await appDbContext.Database.BeginTransactionAsync();
        await appDbContext.Uzytkownik.AddAsync(uzytkownikDoDodania);
        await profilRepository.CreateProfil(new ProfilCreateReqDto(uzytkownik.Id, uzytkownik.Pseudonim));
        await appDbContext.SaveChangesAsync();
        // kończymy transakcję
        await transaction.CommitAsync();
        return new UzytkownikResDto(
            uzytkownik.Id,
            uzytkownik.Login,
            uzytkownik.Haslo,
            uzytkownik.Email,
            uzytkownik.NumerTelefonu,
            uzytkownik.DataUrodzenia,
            new StatusDto(1, "Online")
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
        uzytkownikDoZmiany.StatusId = uzytkownik.Status.Id;
        
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


    public async Task<UzytkownikResDto> UpdateStatus(int id, int idStatus)
    {
        var uzytkownik = await appDbContext.Uzytkownik.FindAsync(id);
        if(uzytkownik == null) throw new Exception("Uzytkownik o id " + id + " nie istnieje");
        uzytkownik.StatusId = idStatus;
        await appDbContext.SaveChangesAsync();
        return await GetUzytkownik(id);
    }
}