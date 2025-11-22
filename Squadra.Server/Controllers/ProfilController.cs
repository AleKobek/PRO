using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.DTO.Profil;
using Squadra.Server.DTO.Status;
using Squadra.Server.Models;
using Squadra.Server.Services;

namespace Squadra.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]

public class ProfilController(IProfilService profilService,
    UserManager<Uzytkownik> userManager) : ControllerBase
{

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<ProfilGetResDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<ProfilGetResDto>>> GetProfile()
    {
        var result = await profilService.GetProfile();
        return Ok(result.Value);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProfilGetResDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<ProfilGetResDto>> GetProfil(int id)
    {
        var result = await profilService.GetProfil(id);
        if(result.StatusCode == 404) return NotFound(result.Errors[0].Message);
        return Ok(result.Value);
    }

    
    // bez awatara!
    [HttpPut("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails),(int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<ActionResult<ProfilGetResDto>> UpdateProfil(int id, [FromBody] ProfilUpdateDto profil)
    {
        // User to ClaimsPrincipal, który ASP.NET Core wypełnia na podstawie cookie (tu Identity cookie)
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");

        // porównujemy id, jeżeli ktoś próbuje zedytować nie swój 
        if (uzytkownik.Id != id)
            return Forbid("Nie możesz edytować profilu innego użytkownika.");
        
        var result = await profilService.UpdateProfil(id, profil);
        switch (result.StatusCode)
        {
            case 404:
                return NotFound(result.Errors[0].Message);
            case 400:
                foreach (var e in result.Errors)
                    ModelState.AddModelError(e.Field ?? string.Empty, e.Message);
                return ValidationProblem();
            default:
                return NoContent();
        }
    }
    
    [HttpPut("{id}/awatar")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails),(int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<ActionResult> UpdateAwatar(int id, [FromForm] IFormFile awatar)
    {
        // User to ClaimsPrincipal, który ASP.NET Core wypełnia na podstawie cookie (tu Identity cookie)
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");

        // porównujemy id, jeżeli ktoś próbuje zedytować nie swój 
        if (uzytkownik.Id != id)
            return Forbid("Nie możesz edytować awatara innego użytkownika.");
        
        var result = await profilService.UpdateAwatar(id, awatar);
        switch (result.StatusCode)
        {
            case 404:
                return NotFound(result.Errors[0].Message);
            case 400:
                foreach (var e in result.Errors)
                    ModelState.AddModelError(e.Field ?? string.Empty, e.Message);
                return ValidationProblem();
            default:
                return NoContent();
        }
    }

    // w reszcie tak samo i not found tak samo i na froncie dostosować
    [HttpGet("{id}/status/baza")]
    [ProducesResponseType(typeof(StatusDto),(int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<StatusDto>> GetStatusZBazyProfilu(int id)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");

        // porównujemy id, jeżeli ktoś próbuje zedytować nie swój 
        if (uzytkownik.Id != id)
            return Forbid("Nie możesz pobrać statusu z bazy dotyczącego profilu innego użytkownika.");
        
        var result = await profilService.GetStatusZBazyProfilu(id);
        if(result.StatusCode == 404) return NotFound(result.Errors[0].Message);
        return Ok(result.Value);
    }

    [HttpGet("{id}/status/wyswietlenie")]
    [ProducesResponseType(typeof(StatusDto),(int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<StatusDto>> GetStatusDoWyswietleniaProfilu(int id)
    {
        var result = await profilService.GetStatusDoWyswietleniaProfilu(id);
        if(result.StatusCode == 404) return NotFound(result.Errors[0].Message);
        return Ok(result.Value);
    }

    [HttpPut("{id}/status")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<ActionResult> UpdateStatus(int id, [FromBody] int idStatus)
    {
        
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");

        // porównujemy id, jeżeli ktoś próbuje zedytować nie swój 
        if (uzytkownik.Id != id)
            return Forbid("Nie możesz edytować statusu innego użytkownika.");
        var result = await profilService.UpdateStatus(id, idStatus);
        
        if (result.StatusCode == 400)
                return NotFound(result.Errors[0].Message);
      
        return NoContent();
    }
}