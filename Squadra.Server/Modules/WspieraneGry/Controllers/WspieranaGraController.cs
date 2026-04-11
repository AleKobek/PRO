using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.Modules.Platformy.Models;
using Squadra.Server.Modules.Uzytkownicy.Models;
using Squadra.Server.Modules.WspieraneGry.DTO;
using Squadra.Server.Modules.WspieraneGry.Models;
using Squadra.Server.Modules.WspieraneGry.Services;

namespace Squadra.Server.Modules.WspieraneGry.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class WspieranaGraController(
    IWspieranaGraService wspieranaGraService,
    UserManager<Uzytkownik> userManager) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Zwraca listę wszystkich wspieranych gier")]
    [ProducesResponseType(typeof(ICollection<WspieranaGra>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> GetWspieraneGry()
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");

        var result = await wspieranaGraService.GetWspieraneGry();
        return result.StatusCode == 200
            ? Ok(result.Value)
            : StatusCode(result.StatusCode, new { errors = result.Errors });
    }

    [HttpGet("{idGry:int}")]
    [EndpointSummary("Zwraca wspieraną grę o podanym id")]
    [ProducesResponseType(typeof(WspieranaGra), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> GetWspieranaGra(int idGry)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");

        var result = await wspieranaGraService.GetWspieranaGra(idGry);
        return result.StatusCode switch
        {
            200 => Ok(result.Value),
            400 => BadRequest(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }

    [HttpGet("mininfo")]
    [EndpointSummary("Zwraca listę wszystkich wspieranych gier z minimalnymi informacjami (id i nazwa)")]
    [ProducesResponseType(typeof(ICollection<WspieranaGra>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult> GetWspieraneGryMinInfo()
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");

        var result = await wspieranaGraService.GetWspieraneGryMinInfo();
        return result.StatusCode == 200
            ? Ok(result.Value)
            : StatusCode(result.StatusCode, new { errors = result.Errors });
    }

    [HttpGet("zplatformami")]
    [EndpointSummary("Zwraca listę wszystkich wspieranych gier wraz z platformami, na których są dostępne")]
    [ProducesResponseType(typeof(ICollection<GraZPlatformaDTO>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult> GetWspieraneGryZPlatformami()
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");

        var result = await wspieranaGraService.GetWspieraneGryZPlatformami();
        return result.StatusCode == 200
            ? Ok(result.Value)
            : StatusCode(result.StatusCode, new { errors = result.Errors });
    }

    [HttpGet("{idGry:int}/platformy")]
    [EndpointSummary("Zwraca listę platform, na których dostępna jest gra o podanym id")]
    [ProducesResponseType(typeof(ICollection<Platforma>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> GetPlatformyGry(int idGry)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await wspieranaGraService.GetPlatformyGry(idGry);
        return result.StatusCode switch
        {
            200 => Ok(result.Value),
            400 => BadRequest(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
}