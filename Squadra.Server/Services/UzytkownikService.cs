using Squadra.Server.DTO.Uzytkownik;
using Squadra.Server.Repositories;

namespace Squadra.Server.Services;

public class UzytkownikService(IUzytkownikRepository uzytkownikRepository) : IUzytkownikService
{
    public async Task<ICollection<UzytkownikResDto>> GetUzytkownicy()
    {
        return await uzytkownikRepository.GetUzytkownicy();
    }

    public async Task<UzytkownikResDto> GetUzytkownik(int id)
    {
        if(id < 1) throw new Exception("Uzytkownik o id " + id + " nie istnieje");
        
        return await uzytkownikRepository.GetUzytkownik(id);
    }

   
    //
    // public async Task<UzytkownikResDto> UpdateUzytkownik(UzytkownikUpdateDto uzytkownik)
    // {
    //     
    // }
    //
    // public async Task DeleteUzytkownik(int id)
    // {
    //     
    // }
}