using Microsoft.AspNetCore.Mvc;
using Squadra.Exceptions;
using Squadra.Services;

namespace Squadra;

[Route("api/[controller]")]
[ApiController]
public class ProfilController(IProfilService profilService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<ProfilGetResDto>> GetProfil(int id)
    {
        try
        {
            return Ok(await profilService.GetProfil(id));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return NotFound(e.Message);
        }
        
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<ProfilUpdateResDto>> UpdateProfil(int id, [FromBody] ProfilUpdateDto profil)
    {
        try
        {
            return Ok(await profilService.UpdateProfil(id, profil));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return NotFound(e.Message);
        }
    }
    
    
}