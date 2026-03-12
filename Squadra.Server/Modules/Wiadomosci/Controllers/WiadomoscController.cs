using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.Modules.Uzytkownicy.Models;
using Squadra.Server.Modules.Wiadomosci.DTO;
using Squadra.Server.Modules.Wiadomosci.Services;

namespace Squadra.Server.Modules.Wiadomosci.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class WiadomoscController(
    IWiadomoscService wiadomoscService,
    IStatystykiCzatuService statykiCzatuService,
    UserManager<Uzytkownik> userManager) : ControllerBase
{
    [HttpGet("{id:int}")]
    [EndpointSummary("Zwraca wiadomość o podanym id")]
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

    [HttpGet("konwersacja/{idZnajomego:int}")]
    [EndpointSummary("Pobiera wszystkie wiadomości między zalogowanym użytkownikiem a uzytkownikiem o podanym id")]
    [ProducesResponseType(typeof(IEnumerable<WiadomoscDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> GetWiadomosci(int idZnajomego)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        var result = await wiadomoscService.GetWiadomosci(uzytkownik.Id, idZnajomego);
        return result.StatusCode switch
        {
            200 => Ok(result.Value),
            403 => StatusCode(StatusCodes.Status403Forbidden, result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }

    [HttpGet("nowe")]
    [EndpointSummary("Zwraca, czy zalogowany uzytkownik ma nowe wiadomości od kogokolwiek")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> CzySaNoweWiadomosci()
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        var result = await statykiCzatuService.CzySaNoweWiadomosciOdZnajomych(uzytkownik.Id);
        return result.StatusCode switch
        {
            200 => Ok(result.Value),
            400 => BadRequest(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
    
    [HttpPost("{idOdbiorcy:int}")]
    [EndpointSummary("Wysyła wiadomość do znajomego")]
    [EndpointDescription("Tworzy nową wiadomość i wysyła ją do wybranego użytkownika")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult> CreateWiadomosc(int idOdbiorcy, WiadomoscCreateDto wiadomosc)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        var result = await wiadomoscService.CreateWiadomosc(idOdbiorcy, wiadomosc, uzytkownik.Id);
        return result.StatusCode switch
        {
            201 => Created(),
            400 => BadRequest(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
}