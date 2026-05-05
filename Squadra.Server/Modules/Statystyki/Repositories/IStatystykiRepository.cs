using Squadra.Server.Modules.Statystyki.DTO;
using Squadra.Server.Modules.Statystyki.Models;

namespace Squadra.Server.Modules.Statystyki.Repositories;

public interface IStatystykiRepository
{
    public Task<Statystyka> GetStatystyka(int idStatystyki);
    public Task<string> GetGodzinyGrania(int idUzytkownika, int idGry);
    public Task<ICollection<CzasRozgrywkiDTO>> GetGodzinyGraniaUzytkownika(int idUzytkownika);
    public Task<WartoscStatystykiDTO?> GetWartoscStatystyki(int idUzytkownika, int idStatystyki);
    public Task<ICollection<StatystykiDoTabelkiDTO>> GetStatystykiZGry(int idUzytkownika, int idGry);
    public Task<bool> UpdateStatystykiUzytkownika(int idUzytkownika, List<StatystykaUzytkownika> noweStatystyki);
    public Task<bool> UsunStatystykiUzytkownika(int idUzytkownika);
    public bool CzySpelniaWymagania(ICollection<WartoscStatystykiDTO> wymagania, ICollection<WartoscStatystykiDTO> statystykiDoSprawdzenia);
    public Task<ICollection<WymaganieDruzynyDoWyswietleniaDto>> GetWymaganiaDruzynyDoWyswietlenia(int idDruzyny);
    public Task<StatystykiDoFormularzaDto> GetStatystykiDoFormularza(int idUzytkownika, int idGry);
}