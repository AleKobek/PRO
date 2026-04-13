using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.Modules.Platformy.DTO;
using Squadra.Server.Modules.Platformy.Models;
using Squadra.Server.Modules.Platformy.Services;
using Squadra.Server.Modules.Uzytkownicy.Models;

namespace Squadra.Server.Modules.Platformy.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PlatformaController(
    IPlatformaService platformaService,
    UserManager<Uzytkownik> userManager) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Zwraca dane wszystkich platform")]
    [ProducesResponseType(typeof(IEnumerable<Platforma>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<IEnumerable<Platforma>>> GetPlatformy()
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");

        var result = await platformaService.GetPlatformy();
        return result.StatusCode == 200
            ? Ok(result.Value)
            : StatusCode(result.StatusCode, new { errors = result.Errors });
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Zwraca dane platformy o podanym id")]
    [ProducesResponseType(typeof(Platforma), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<Platforma>> GetPlatforma(int id)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");

        var result = await platformaService.GetPlatforma(id);
        return result.StatusCode switch
        {
            200 => Ok(result.Value),
            400 => BadRequest(result.Errors[0].Message),
            401 => Unauthorized(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }

    [HttpGet("uzytkownik/{idUzytkownika:int}")]
    [EndpointSummary("Zwraca dane platform użytkownika o podanym id")]
    [ProducesResponseType(typeof(IEnumerable<PlatformaUzytkownikaDTO>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IEnumerable<PlatformaUzytkownikaDTO>>> GetPlatformyUzytkownika(int idUzytkownika)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");

        var result = await platformaService.GetPlatformyUzytkownika(idUzytkownika);
        return result.StatusCode switch
        {
            200 => Ok(result.Value),
            400 => BadRequest(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }

    [HttpPost("{id:int}")]
    [EndpointSummary("Tworzy nową platformę")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreatePlatforma(int id, string nazwa, IFormFile logo)
    {
        var result = await platformaService.CreatePlatforma(id, nazwa, logo);
        return result.StatusCode switch
        {
            201 => CreatedAtAction(nameof(GetPlatforma), new { id }, null),
            400 => BadRequest(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
}