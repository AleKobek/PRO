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
    [ProducesResponseType(404)]
    public async Task<ActionResult<DruzynaSzczegolyDto>> GetDruzynaSzczegoly(int idDruzyny)
    {
        var result = await druzynyService.PodajSzczegolyDruzyny(idDruzyny);
        return result.StatusCode switch
        {
            200 => Ok(result.Value),
            400 => BadRequest(result.Errors[0].Message),
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
}