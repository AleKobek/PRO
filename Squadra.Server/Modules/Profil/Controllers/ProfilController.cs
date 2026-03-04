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

public class ProfilController(
    IProfilService profilService,
    UserManager<Uzytkownik> userManager) : ControllerBase
{

    [HttpGet("admin")]
    [EndpointSummary("Zwraca dane wszystkich istniejących profilów (tylko dla admina")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<ProfilGetResDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<ProfilGetResDto>>> GetProfile()
    {
        var result = await profilService.GetProfile();
        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Zwraca dane profilu o podanym id.")]
    [ProducesResponseType(typeof(ProfilGetResDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<ProfilGetResDto>> GetProfil(int id)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        var result = await profilService.GetProfil(id);
        if (result.StatusCode == 404) return NotFound(result.Errors[0].Message);
        return Ok(result.Value);
    }
    
    [HttpPut]
    [EndpointSummary("Aktualizuje dane profilu zalogowanego użytkownika")]
    [EndpointDescription("Nie zawiera w sobie aktualizacji awatara.")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails),(int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> UpdateProfil([FromBody] ProfilUpdateDto profil)
    {
        // User to ClaimsPrincipal, który ASP.NET Core wypełnia na podstawie cookie (tu Identity cookie)
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await profilService.UpdateProfil(uzytkownik.Id, profil);
        switch (result.StatusCode)
        {
            case 204: return NoContent();
            case 404:
                return NotFound(result.Errors[0].Message);
            case 400:
                foreach (var e in result.Errors)
                    ModelState.AddModelError(e.Field ?? string.Empty, e.Message);
                return ValidationProblem();
            default:
                return StatusCode(result.StatusCode, new { errors = result.Errors });
        }
    }
    
    [EndpointSummary("Aktualizuje awatar profilu zalogowanego użytkownika.")]
    [HttpPut("awatar")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails),(int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> UpdateAwatar(IFormFile awatar)
    {
        // User to ClaimsPrincipal, który ASP.NET Core wypełnia na podstawie cookie (tu Identity cookie)
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await profilService.UpdateAwatar(uzytkownik.Id, awatar);
        switch (result.StatusCode)
        {
            case 204: return NoContent();
            case 404:
                return NotFound(result.Errors[0].Message);
            case 400:
                foreach (var e in result.Errors)
                    ModelState.AddModelError(e.Field ?? string.Empty, e.Message);
                return ValidationProblem();
            default:
                return StatusCode(result.StatusCode, new { errors = result.Errors });
        }
    }
    
    [HttpGet("status")]
    [EndpointSummary("Zwraca status profilu zalogowanego użytkownika, pobrany bezpośrednio z bazy.")]
    [EndpointDescription(" Na froncie jest dostępne dla właściciela profilu.")]
    [ProducesResponseType(typeof(StatusDto),(int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<StatusDto>> GetStatusZBazyProfilu()
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await profilService.GetStatusZBazyProfilu(uzytkownik.Id);
        if(result.StatusCode == 404) return NotFound(result.Errors[0].Message);
        return Ok(result.Value);
    }


    [HttpGet("{id:int}/status")]
    [EndpointSummary("Zwraca status danego profilu w formie przeznaczonej dla innych użytkowników.")]
    [EndpointDescription("Może zwrócić status \"offline\", nieistniejący w bazie, jeżeli właściciel profilu nie " +
                         "miał otwartego okna przeglądarki przez kilka minut lub ma ustawiony status \"niedostępny\".")]
    [ProducesResponseType(typeof(StatusDto),(int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<StatusDto>> GetStatusDoWyswietleniaProfilu(int id)
    {
        var result = await profilService.GetStatusDoWyswietleniaProfilu(id);
        if(result.StatusCode == 404) return NotFound(result.Errors[0].Message);
        return Ok(result.Value);
    }

    [EndpointSummary("Aktualizuje status profilu zalogowanego użytkownika.")]
    [HttpPut("status")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult> UpdateStatus([FromBody] int idStatus)
    {
        
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await profilService.UpdateStatus(uzytkownik.Id, idStatus);
        
        if (result.StatusCode == 400)
                return NotFound(result.Errors[0].Message);
      
        return NoContent();
    }
}