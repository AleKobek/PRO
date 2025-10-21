using Squadra.Server.DTO.Profil;
using Squadra.Server.DTO.Status;

namespace Squadra.Server.Services;

public interface IProfilService
{
    public Task<ProfilGetResDto> GetProfil(int id);

    public Task<ProfilUpdateResDto> UpdateProfil(int id, ProfilUpdateDto profil);

    public Task<StatusDto> GetStatusZBazyProfilu(int id);

    public Task<StatusDto> GetStatusDoWyswietleniaProfilu(int id);

    public Task<ProfilGetResDto> UpdateStatus(int id, int idStatus);

}