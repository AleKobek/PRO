namespace Squadra;

public interface IProfilRepository
{
    public Task<ICollection<ProfilGetDto>> GetProfile();

    public Task<ProfilGetDto> GetProfilUzytkownika(int id);

    public Task<ProfilGetDto> UpdateProfil(ProfilUpdateDto profil);

    public Task<ProfilGetDto> CreateProfil(ProfilCreateDto profil);
    
    public Task DeleteProfil(int id);
}