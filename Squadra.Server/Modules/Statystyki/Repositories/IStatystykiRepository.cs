using Squadra.Server.Modules.Statystyki.DTO;
using Squadra.Server.Modules.Statystyki.Models;

namespace Squadra.Server.Modules.Statystyki.Repositories;

public interface IStatystykiRepository
{
    public Task<string> GetGodzinyGrania(int idUzytkownika, int idGry);
    public Task<ICollection<CzasRozgrywkiDTO>> GetGodzinyGraniaUzytkownika(int idUzytkownika);
    public Task<string?> GetWartoscStatystyki(int idUzytkownika, int idStatystyki);
    public Task<ICollection<StatystykaDTO>> GetStatystykiZGry(int idUzytkownika, int idGry);
    public Task<bool> UpdateStatystykiUzytkownika(int idUzytkownika, List<StatystykaUzytkownika> noweStatystyki);
    public Task<bool> UsunStatystykiUzytkownika(int idUzytkownika);
}