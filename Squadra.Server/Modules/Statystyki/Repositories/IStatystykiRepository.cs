using Squadra.Server.Modules.Statystyki.DTO;
using Squadra.Server.Modules.Statystyki.Models;

namespace Squadra.Server.Modules.Statystyki.Repositories;

public interface IStatystykiRepository
{
    public Task<Statystyka> GetStatystyka(int idStatystyki);
    public Task<string> GetGodzinyGrania(int idUzytkownika, int idGry);
    public Task<ICollection<CzasRozgrywkiDTO>> GetGodzinyGraniaUzytkownika(int idUzytkownika);
    public Task<WartoscStatystykiDTO?> GetWartoscStatystyki(int idUzytkownika, int idStatystyki);
    public Task<ICollection<StatystykiDoTabelkiDTO>> GetStatystykiUzytkownikaZGry(int idUzytkownika, int idGry);
    public Task<ICollection<Statystyka>> GetStatystykiZGry(int idGry);

    public Task<ICollection<RangiStatystykiDto>> GetMniejszeLubRowneRangiGryUzytkownika(int idGry, int idUzytkownika);
    public Task<bool> UpdateStatystykiUzytkownika(int idUzytkownika, List<StatystykaUzytkownika> noweStatystyki);
    public Task<bool> UsunStatystykiUzytkownika(int idUzytkownika);
    public bool CzySpelniaWymagania(ICollection<WartoscStatystykiDTO> wymagania, ICollection<WartoscStatystykiDTO> statystykiDoSprawdzenia);
    public Task<ICollection<WymaganieDruzynyDoWyswietleniaDto>> GetWymaganiaDruzynyDoWyswietlenia(int idDruzyny);
    public Task<ICollection<RangiStatystykiDto>> GetRangiGry(int idGry);
    public Task<ICollection<Rola>> GetRoleGry(int idGry);
    public Task<ICollection<Rola>> GetRole();
    public Task<Rola> GetRola(int idRoli);
    public ICollection<int> FiltrujNieistniejaceStatystyki(ICollection<int> idStatystyk);
    public Task<bool> UsunWymaganeStatystykiDruzyny(int idDruzyny);
}