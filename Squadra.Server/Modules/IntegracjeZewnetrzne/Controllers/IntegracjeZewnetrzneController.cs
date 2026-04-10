using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.Modules.IntegracjeZewnetrzne.Services;
using Squadra.Server.Modules.Uzytkownicy.Models;

namespace Squadra.Server.Modules.IntegracjeZewnetrzne.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class IntegracjeZewnetrzneController(
    IIntegracjeZewnetrzneService integracjeZewnetrzneService,
    UserManager<Uzytkownik> userManager) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Łączy konto uzytkownika z zewnętrznym serwisem na podstawie loginu i hasła")]
    [EndpointDescription("W przypadku sukcesu zewnętrzne id użytkownika jest ustawione na id z tamtego serwisu")]
    public async Task<IActionResult> ZintegrujKonto(string login, string haslo)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");

        var result = await integracjeZewnetrzneService.ZintegrujKonto(uzytkownik.Id, login, haslo);
        return result.StatusCode switch
        {
            200 => Ok(result.Value),
            400 => BadRequest(result.Errors[0].Message),
            401 => Unauthorized(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }

    [HttpPost("przerwij")]
    [EndpointSummary("Odłącza konto użytkownika od zewnętrznego serwisu")]
    public async Task<IActionResult> PrzerwijIntegracje()
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");

        var result = await integracjeZewnetrzneService.PrzerwijIntegracjeUzytkownika(uzytkownik.Id);
        return result.StatusCode switch
        {
            200 => Ok(result.Value),
            400 => BadRequest(result.Errors[0].Message),
            401 => Unauthorized(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
}