using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Uzytkownicy.DTO.Uzytkownik;

namespace Squadra.Server.Modules.Uzytkownicy.Services;

public interface IUzytkownikService
{
    public Task<ServiceResult<UzytkownikResDto>> GetUzytkownik(int id);

    public Task<ServiceResult<UzytkownikResDto>> GetUzytkownik(string login);
    public Task<ServiceResult<DateTime?>> GetOstatniaAktywnoscUzytkownika(int id);
    public Task<ServiceResult<bool>> CzyUzytkownikJestAdminem(int id);
    public Task<ServiceResult<bool>> CzyUzytkownikMaZintegrowaneKonto(int id);
    Task<ServiceResult<bool>> CreateUzytkownik(UzytkownikCreateDto uzytkownik);

    public Task<ServiceResult<bool>> UpdateUzytkownik(int id, UzytkownikUpdateDto dto);

    public Task<ServiceResult<bool>> UpdateHaslo(int idUzytkownika, string stareHaslo, string noweHaslo);
    public Task<ServiceResult<bool>> UpdateDaneKontaNaZewnetrznymSerwisie(int id, int? idNaZewnetrznymSerwisie, string? loginNaZewnetrznymSerwisie);
    public Task<ServiceResult<bool>> DeleteUzytkownik(int id);

}