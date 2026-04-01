using Squadra.Server.Modules.Statystyki.DTO;

namespace Squadra.Server.Modules.Statystyki.Repositories;

public interface IStatystykiRepository
{
    public Task<string> GetGodzinyGrania(int idUzytkownika, int idGry);
    public Task<string?> GetWartoscStatystyki(int idUzytkownika, int idStatystyki);
    public Task<ICollection<StatystykaDTO>> GetStatystykiZGry(int idUzytkownika, int idGry);
}