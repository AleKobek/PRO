using Squadra.Server.Modules.Profile.DTO.Profil;
using Squadra.Server.Modules.Profile.DTO.Status;
using Squadra.Server.Modules.Shared.Services;

namespace Squadra.Server.Modules.Profile.Services;

public interface IProfilService
{

    public Task<ServiceResult<ICollection<ProfilGetResDto>>> GetProfile();
    
    public Task<ServiceResult<ProfilGetResDto>> GetProfil(int id);

    public Task<ServiceResult<ProfilGetResDto>> GetProfil(string login);

    public Task<ServiceResult<bool>> UpdateProfil(int id, ProfilUpdateDto profil);

    public Task<ServiceResult<bool>> UpdateAwatar(int id, IFormFile awatar);

    public Task<ServiceResult<StatusDto>> GetStatusZBazyProfilu(int id);

    public Task<ServiceResult<StatusDto>> GetStatusDoWyswietleniaProfilu(int id);

    public Task<ServiceResult<StatusDto>> UpdateStatus(int id, int idStatus);

}