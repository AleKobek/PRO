using Microsoft.AspNetCore.Mvc;
using Squadra.Server.DTO.Profil;
using Squadra.Server.DTO.Status;
using Squadra.Server.Exceptions;
using Squadra.Server.Services;

namespace Squadra.Server.Controllers;

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
    
    public async Task<ActionResult<StatusDto>> GetStatusZBazyProfilu(int id)
    {
        try
        {
            return Ok(await profilService.GetStatusZBazyProfilu(id));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpGet("{id}/status/wyswietlenie")]
    public async Task<ActionResult<StatusDto>> GetStatusDoWyswietleniaProfilu(int id)
    {
        try
        {
            return Ok(await profilService.GetStatusDoWyswietleniaProfilu(id));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPut("{id}/status")]
    private async Task<ActionResult<ProfilGetResDto>> UpdateStatus(int id, [FromBody] int idStatus)
    {
        try
        {
            return Ok(await profilService.UpdateStatus(id, idStatus));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return NotFound(e.Message);
        }
    }
}