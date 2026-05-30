using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.Modules.Drużyny.DTO;
using Squadra.Server.Modules.Drużyny.Services;
using Squadra.Server.Modules.Uzytkownicy.Models;

namespace Squadra.Server.Modules.Drużyny.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DruzynaController(IDruzynyService druzynyService, UserManager<Uzytkownik> userManager) : ControllerBase
{
     
    [HttpGet("tabelka/{idUzytkownika:int}")]
    [EndpointSummary("Zwraca wszystkie drużyny użytkownika, w formacie potrzebnym do wyświetlenia ich w tabelce na stronie głównej.")]
    [ProducesResponseType(typeof(ICollection<DruzynaDoTabelkiDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ICollection<DruzynaDoTabelkiDto>>> GetWszystkieDruzynyUzytkownikaDoTabelki(int idUzytkownika)
    {
        var result = await druzynyService.GetWszystkieDruzynyUzytkownikaDoTabelki(idUzytkownika);
        return result.StatusCode switch
        {
            200 => Ok(result.Value),
            400 => BadRequest(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
    
    [HttpGet("szczegoly/{idDruzyny:int}")]
    [EndpointSummary("Zwraca szczegółowe dane drużyny, potrzebne do wyświetlenia jej na stronie drużyny.")]
    [ProducesResponseType(typeof(DruzynaSzczegolyDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<DruzynaSzczegolyDto>> GetDruzynaSzczegoly(int idDruzyny)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await druzynyService.PodajSzczegolyDruzyny(idDruzyny, uzytkownik.Id);
        return result.StatusCode switch
        {
            200 => Ok(result.Value),
            400 => BadRequest(result.Errors[0].Message),
            403 => StatusCode(403, result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
    [HttpGet("formularz/ze-statystykami/{idGry:int}")]
    [EndpointSummary("Zwraca dane potrzebne do wyświetlenia formularza tworzenia drużyny, ze statystykami.")]
    [EndpointDescription("Podajemy uzytkownika, który tworzy drużynę, aby zwrócić jego statystyki, które mogą być przydatne przy tworzeniu drużyny. Podajemy też id gry, aby zwrócić tylko statystyki z tej gry.")]
    [ProducesResponseType(typeof(DaneDoFormularzaDruzynyZeStatystykamiDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<DaneDoFormularzaDruzynyZeStatystykamiDto>> GetDaneDoFormularzaDruzynyZeStatystykami(int idGry)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await druzynyService.GetDaneDoFormularzaDruzynyZeStatystykami(idGry, uzytkownik.Id);
        return result.StatusCode switch
        {
            200 => Ok(result.Value),
            400 => BadRequest(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
    
     [HttpGet("formularz/bez-statystyk/{idGry:int}")]
     [EndpointSummary("Zwraca dane potrzebne do wyświetlenia formularza tworzenia drużyny, bez statystyk.")]
     [EndpointDescription("Zwraca dane potrzebne do wyświetlenia formularza tworzenia drużyny, bez statystyk. Podajemy id gry, aby zwrócić tylko statystyki z tej gry.")]
     [ProducesResponseType(typeof(DaneDoFormularzaDruzynyBezStatystykDto), 200)]
     [ProducesResponseType(400)]
     [ProducesResponseType(404)]
     public async Task<ActionResult<DaneDoFormularzaDruzynyBezStatystykDto>> GetDaneDoFormularzaDruzynyBezStatystyk(int idGry)
    {
        var result = await druzynyService.GetDaneDoFormularzaDruzynyBezStatystyk(idGry);
        return result.StatusCode switch
        {
            200 => Ok(result.Value),
            400 => BadRequest(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
     
    [HttpGet("formularz/wyszukiwanie")]
    [EndpointSummary("Zwraca dane potrzebne do wyświetlenia formularza wyszukiwania drużyny")]
    [EndpointDescription("Zwraca dane potrzebne do wyświetlenia formularza wyszukiwania")]
    [ProducesResponseType(typeof(DaneDoFormularzaDruzynyBezStatystykDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<DaneDoFormularzaWyszukiwaniaDruzyny>> GetDaneDoFormularzaWyszukiwaniaDruzyny()
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await druzynyService.GetDaneDoFormularzaWyszukiwaniaDruzyny(uzytkownik.Id);
        return result.StatusCode switch
        {
            200 => Ok(result.Value),
            400 => BadRequest(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
    
    [HttpGet("nastroje-rozgrywki")]
    [EndpointSummary("Zwraca nastroje rozgrywki, które można wybrać przy tworzeniu drużyny.")]
    [EndpointDescription("Zwraca nastroje rozgrywki, które można wybrać przy tworzeniu i edytowaniu drużyny.")]
    [ProducesResponseType(typeof(IEnumerable<NastrojRozgrywkiDto>), 200)]
    public async Task<ActionResult<IEnumerable<NastrojRozgrywkiDto>>> GetNastrojeRozgrywki()
    {
        var result = await druzynyService.GetNastrojeRozgrywki();
        return result.StatusCode switch
        {
            200 => Ok(result.Value),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
    
    [HttpPost]
    [EndpointSummary("Tworzy drużynę")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult> CreateDruzyna(CreateDruzynaReqDto dto)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null) return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await druzynyService.StworzDruzyne(dto, uzytkownik.Id);

        switch (result.StatusCode)
        {
            // Dla 400 u nas zwracamy ValidationProblem, ponieważ błędy dotyczą konkretnych pól
            case 201:
                return Created();
            case 400:
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError(e.Field ?? string.Empty, e.Message);
                return ValidationProblem();
            }
            case 404:
                return NotFound(new { errors = result.Errors });
            // coś jeszcze innego
            default:
                return StatusCode(result.StatusCode, new { errors = result.Errors });
        }
    }
    
    [HttpPut("opuszczanie/{idDruzyny:int}")]
    [EndpointSummary("Pozwala użytkownikowi opuścić drużynę, do której należy.")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult> OpuscDruzyne(int idDruzyny)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null) return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await druzynyService.OpuscDruzyne(idDruzyny, uzytkownik.Id);
        return result.StatusCode switch
        {
            204 => NoContent(),
            400 => BadRequest(result.Errors[0].Message),
            403 => StatusCode(403, result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
    [HttpPut("miejsce/{idmiejscaWDruzynie:int}")]
    [EndpointSummary("Pozwala opróżnić dane miejsce w drużynie.")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult> OproznijMiejsceWDruzynie(int idmiejscaWDruzynie, int idUzytkownika)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null) return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await druzynyService.OproznijMiejsceWDruzynie(idmiejscaWDruzynie, uzytkownik.Id);
        return result.StatusCode switch
        {
            204 => NoContent(),
            400 => BadRequest(result.Errors[0].Message),
            403 => StatusCode(403, result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
    
    [HttpDelete("{idDruzyny:int}")]
    [EndpointSummary("Usuwa drużynę")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<ActionResult> DeleteDruzyna(int idDruzyny)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null) return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await druzynyService.UsunDruzyne(idDruzyny, uzytkownik.Id);
        return result.StatusCode switch
        {
            204 => NoContent(),
            400 => BadRequest(result.Errors[0].Message),
            403 => StatusCode(403, result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
    
    [HttpPut("{idDruzyny:int}")]
    [EndpointSummary("Aktualizuje dane drużyny")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<ActionResult> UpdateDruzyna(int idDruzyny, DruzynaUpdateDto dto)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null) return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await druzynyService.UpdateDruzyna(idDruzyny, uzytkownik.Id, dto);
        return result.StatusCode switch
        {
            204 => NoContent(),
            400 => BadRequest(result.Errors[0].Message),
            403 => StatusCode(403, result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
}