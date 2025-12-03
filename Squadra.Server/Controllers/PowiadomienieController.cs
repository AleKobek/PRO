using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.DTO.Powiadomienie;
using Squadra.Server.Models;
using Squadra.Server.Services;

namespace Squadra.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PowiadomienieController(IPowiadomienieService powiadomienieService,
    UserManager<Uzytkownik> userManager) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PowiadomienieDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<IEnumerable<PowiadomienieDto>>> GetPowiadomienia()
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        var result = await powiadomienieService.GetPowiadomieniaUzytkownika(uzytkownik.Id);
        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PowiadomienieDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<PowiadomienieDto>> GetPowiadomienie(int id)
    {
        var result = await powiadomienieService.GetPowiadomienie(id, User);
        return result.StatusCode switch
        {
            401 => Unauthorized(result.Errors[0].Message),
            403 => Forbid(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => Ok(result.Value)
        };
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<ActionResult> RozpatrzPowiadomienie(int id, bool? czyZaakceptowane)
    {
        var result = await powiadomienieService.RozpatrzPowiadomienie(id, czyZaakceptowane, User);
        return result.StatusCode switch
        {
            400 => BadRequest(result.Errors[0].Message),
            401 => Unauthorized(result.Errors[0].Message),
            403 => Forbid(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => NoContent()
        };
    }
    
    [HttpPost("{idUzytkownika:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    
    public async Task<ActionResult> StworzPowiadomienieSystemowe(int idUzytkownika, string tresc)
    {
        var result = await powiadomienieService.CreatePowiadomienie(new PowiadomienieCreateDto(
            1, idUzytkownika, null, tresc));

        return result.StatusCode switch
        {
            404 => NotFound(result.Errors[0].Message),
            _ => NoContent()
        };
    }
}