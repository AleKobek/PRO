namespace Squadra.Services;

public interface IProfilService
{
    public Task<ProfilGetResDto> GetProfil(int id);

    public Task<ProfilUpdateResDto> UpdateProfil(int id, ProfilUpdateDto profil);

}