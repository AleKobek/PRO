using Microsoft.AspNetCore.Mvc;
using Squadra.Services;

namespace Squadra;

[Route("api/[controller]")]
[ApiController]
public class ProfilController(IProfilService profilService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ProfilGetResDto> GetProfil(int id)
    {
        return await profilService.GetProfil(id);
    }
    
    [HttpPut]
    public async Task<ProfilUpdateResDto> UpdateProfil([FromBody] ProfilUpdateDto profil)
    {
        return await profilService.UpdateProfil(profil);
    }
    
    
}