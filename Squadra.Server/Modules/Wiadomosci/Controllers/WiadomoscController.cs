using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.DTO.Wiadomosc;
using Squadra.Server.Models;
using Squadra.Server.Services;

namespace Squadra.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class WiadomoscController(
    IWiadomoscService wiadomoscService,
    UserManager<Uzytkownik> userManager) : ControllerBase
{
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(WiadomoscDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> GetWiadomosc(int id)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");

        var result = await wiadomoscService.GetWiadomosc(id, uzytkownik.Id);
        return result.StatusCode switch
        {
            200 => Ok(result.Value),
            403 => StatusCode(StatusCodes.Status403Forbidden, result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }

    [HttpGet("konwersacja/{idOdbiorcy:int}")]
    [ProducesResponseType(typeof(IEnumerable<WiadomoscDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> GetWiadomosci(int idOdbiorcy)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        var result = await wiadomoscService.GetWiadomosci(uzytkownik.Id, idOdbiorcy);
        return result.StatusCode switch
        {
            200 => Ok(result.Value),
            403 => StatusCode(StatusCodes.Status403Forbidden, result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult> CreateWiadomosc(WiadomoscCreateDto wiadomosc)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        var result = await wiadomoscService.CreateWiadomosc(wiadomosc, uzytkownik.Id);
        return result.StatusCode switch
        {
            201 => Created(),
            400 => BadRequest(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
}