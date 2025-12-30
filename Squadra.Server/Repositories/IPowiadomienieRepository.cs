using Squadra.Server.DTO.Powiadomienie;

namespace Squadra.Server.Repositories;

public interface IPowiadomienieRepository
{
    public Task<PowiadomienieDto> GetPowiadomienie(int id);
    public Task<ICollection<PowiadomienieDto>> GetPowiadomieniaUzytkownika(int idUzytkownika);
    public Task<bool> CreatePowiadomienie(PowiadomienieCreateDto powiadomienie);
    public Task<bool> DeletePowiadomienie(int id);
    public Task<bool> DeletePowiadomieniaUzytkownika(int idUzytkownika);
    public Task<string> GetNazwaTypuPowiadomienia(int idTypuPowiadomienia);
}