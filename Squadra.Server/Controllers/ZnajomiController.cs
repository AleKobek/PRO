using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.DTO.Powiadomienie;
using Squadra.Server.DTO.Profil;
using Squadra.Server.Models;
using Squadra.Server.Services;

namespace Squadra.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ZnajomiController(IZnajomiService znajomiService, 
    IPowiadomienieService powiadomienieService, UserManager<Uzytkownik> userManager) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProfilGetResDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IEnumerable<ProfilGetResDto>>> GetZnajomi()
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        var result = await znajomiService.GetZnajomiUzytkownika(uzytkownik.Id);
        if (result.StatusCode == 404) return NotFound(result.Errors[0].Message);
        return Ok(result.Value);
    }
    
    [HttpDelete("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<bool>> DeleteZnajomego(int id)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await znajomiService.DeleteZnajomosc(uzytkownik.Id, id);
        
        if(result.StatusCode == 404) return NotFound(result.Errors[0].Message);
        
        var resultPowiadomienia = await powiadomienieService.CreatePowiadomienie(new PowiadomienieCreateDto(
            5,
            id, // wysyłamy to temu, który został usunięty
            uzytkownik.Id,
            null
        ));

        return resultPowiadomienia.StatusCode switch
        {
            204 => NoContent(),
            404 => NotFound(resultPowiadomienia.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
}