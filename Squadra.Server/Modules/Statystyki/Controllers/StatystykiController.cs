using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.Modules.Statystyki.DTO;
using Squadra.Server.Modules.Statystyki.Services;
using Squadra.Server.Modules.Uzytkownicy.Models;

namespace Squadra.Server.Modules.Statystyki.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class StatystykiController(
    IStatystykiService statystykiService,
    UserManager<Uzytkownik> userManager) : ControllerBase
{
    [HttpGet("{idUzytkownika:int}/{idGry:int}")] // to GET, więc oba w ścieżce
    [EndpointSummary("Pobiera wszystkie statystyki użytkownika o podanym id związane z grą o podanym id")]
    [ProducesResponseType(typeof(ICollection<StatystykaDTO>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> GetStatystykiZGry(int idUzytkownika, int idGry)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await statystykiService.GetStatystykiZGry(idUzytkownika, idGry);
        return result.StatusCode switch
        {
            200 => Ok(result.Value),
            400 => BadRequest(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
}