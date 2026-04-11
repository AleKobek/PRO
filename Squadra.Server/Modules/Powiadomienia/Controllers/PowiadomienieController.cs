using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.Modules.Powiadomienia.DTO;
using Squadra.Server.Modules.Powiadomienia.Services;
using Squadra.Server.Modules.Uzytkownicy.Models;

namespace Squadra.Server.Modules.Powiadomienia.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PowiadomienieController(IPowiadomienieService powiadomienieService,
    UserManager<Uzytkownik> userManager) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Zwraca dane wszystkich powiadomień zalogowanego użytkownika")]
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
    [EndpointSummary("Zwraca dane powiadomienia o podanym id")]
    [ProducesResponseType(typeof(PowiadomienieDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<PowiadomienieDto>> GetPowiadomienie(int id)
    {
        var result = await powiadomienieService.GetPowiadomienie(id, User);
        return result.StatusCode switch
        {
            200 => Ok(result.Value),
            400 => BadRequest(result.Errors[0].Message),
            401 => Unauthorized(result.Errors[0].Message),
            403 => StatusCode(StatusCodes.Status403Forbidden,result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }

    [HttpPut("{id:int}")]
    [EndpointSummary("Rozpatruje powiadomienie o podanym id")]
    [EndpointDescription("W przypadku powiadomień wymagających akceptacji, należy w body przekazać czy zostało zaakceptowane czy odrzucone.")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<ActionResult> RozpatrzPowiadomienie(int id, [FromBody] bool? czyZaakceptowane)
    {
        var result = await powiadomienieService.RozpatrzPowiadomienie(id, czyZaakceptowane, User);
        return result.StatusCode switch
        {
            204 => NoContent(),
            400 => BadRequest(result.Errors[0].Message),
            401 => Unauthorized(result.Errors[0].Message),
            403 => StatusCode(StatusCodes.Status403Forbidden,result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
    
    [HttpPost("{idUzytkownika:int}")]
    [Authorize(Roles = "Admin")]
    [EndpointSummary("Tworzy powiadomienie systemowe dla użytkownika o podanym id")]
    [EndpointDescription("Tylko dla administratorów. Powiadomienie systemowe to takie, które nie jest związane z żadnym konkretnym działaniem użytkownika, ale ma na celu przekazanie mu ważnej informacji. Przykładowo, może to być powiadomienie o planowanej przerwie technicznej, aktualizacji regulaminu czy innych istotnych zmianach dotyczących korzystania z aplikacji.")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    
    public async Task<ActionResult> StworzPowiadomienieSystemowe(int idUzytkownika, string tresc)
    {
        var result = await powiadomienieService.CreatePowiadomienie(new PowiadomienieCreateDto(
            1, idUzytkownika, null, null, tresc));

        return result.StatusCode switch
        {
            204 => NoContent(),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
    
    [HttpPost("zaproszenie/znajomi")]
    [EndpointSummary("Wysyła zaproszenie do znajomych, skierowane do użytkownika o podanym loginie")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> WyslijZaproszenieDoZnajomychPoLoginie([FromBody]string loginZapraszanegoUzytkownika)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await powiadomienieService.WyslijZaproszenieDoZnajomychPoLoginie(uzytkownik.Id, loginZapraszanegoUzytkownika);
        return result.StatusCode switch
        {
            204 => NoContent(),
            400 => BadRequest(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            409 => Conflict(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
    
    [HttpPost("zaproszenie/znajomi/{idZapraszanegoUzytkownika:int}")]
    [EndpointSummary("Wysyła zaproszenie do znajomych, skierowane do użytkownika o podanym id")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> WyslijZaproszenieDoZnajomychPoId(int idZapraszanegoUzytkownika)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await powiadomienieService.WyslijZaproszenieDoZnajomychPoId(uzytkownik.Id, idZapraszanegoUzytkownika);
        return result.StatusCode switch
        {
            204 => NoContent(),
            400 => BadRequest(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            409 => Conflict(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
}