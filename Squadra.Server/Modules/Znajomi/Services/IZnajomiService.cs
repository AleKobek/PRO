using Squadra.Server.DTO.Profil;
using Squadra.Server.Models;
using Squadra.Server.Modules.Znajomi.DTO;

namespace Squadra.Server.Services;

public interface IZnajomiService
{
    public Task<ServiceResult<ICollection<Znajomi>>> GetZnajomiUzytkownika(int id);
    public Task<ServiceResult<ICollection<ZnajomyDoListyDto>>> GetZnajomiDoListyUzytkownika(int id);
    public Task<ServiceResult<DateTime?>> GetDataOstatniegoOtwarciaCzatu(int idSprawdzajacego, int idZnajomego);
    public Task<ServiceResult<bool>> CreateZnajomosc(int idUzytkownika1, int idUzytkownika2);
    public Task<ServiceResult<bool>> DeleteZnajomosc(int idUzytkownikaInicjujacego, int idUzytkownika2);
    public Task<ServiceResult<bool>> ZaktualizujOstatnieOtwarcieCzatu(int idOtwierajacego, int idZnajomego);
}