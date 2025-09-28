namespace Squadra;

public interface IProfilRepository
{
    public Task<ICollection<ProfilGetResDto>> GetProfile();

    public Task<ProfilGetResDto> GetProfilUzytkownika(int id);

    public Task<ProfilGetResDto> UpdateProfil(int id, ProfilUpdateDto profil);

    public Task<ProfilGetResDto> CreateProfil(ProfilCreateReqDto profil);
    
    public Task DeleteProfil(int id);
}