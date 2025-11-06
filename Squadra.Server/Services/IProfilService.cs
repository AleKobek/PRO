using Squadra.Server.DTO.Profil;
using Squadra.Server.DTO.Status;

namespace Squadra.Server.Services;

public interface IProfilService
{
    public Task<ServiceResult<ProfilGetResDto>> GetProfil(int id);

    public Task<ServiceResult<ProfilGetResDto>> UpdateProfil(int id, ProfilUpdateDto profil);

    public Task<ServiceResult<StatusDto>> GetStatusZBazyProfilu(int id);

    public Task<ServiceResult<StatusDto>> GetStatusDoWyswietleniaProfilu(int id);

    public Task<ServiceResult<ProfilGetResDto>> UpdateStatus(int id, int idStatus);

}