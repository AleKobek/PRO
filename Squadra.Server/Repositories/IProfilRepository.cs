using Squadra.Server.DTO.Profil;
using Squadra.Server.DTO.Status;

namespace Squadra.Server.Repositories;

public interface IProfilRepository
{
    public Task<ICollection<ProfilGetResDto>> GetProfile();

    public Task<ProfilGetResDto> GetProfilUzytkownika(int id);

    public Task<bool> UpdateProfil(int id, ProfilUpdateDto profil);
    public Task<bool> UpdateAwatar(int id, byte[] awatar);

    public Task<ProfilGetResDto> CreateProfil(ProfilCreateReqDto profil);
    
    public Task DeleteProfil(int id);

    public Task<StatusDto> GetStatusProfilu(int id);
    
    public Task<StatusDto> UpdateStatus(int id, int idStatus);
}