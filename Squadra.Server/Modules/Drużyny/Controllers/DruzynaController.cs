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
    
    [HttpPost]
    [EndpointSummary("Tworzy drużynę")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult> CreateDruzyna(CreateDruzynaReqDto dto)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        
        // var result = await druzynyService.CreateDruzyna(dto, uzytkownik.Id);
        // return result.StatusCode switch
        // {
        //     201 => Created(),
        //     400 => BadRequest(result.Errors[0].Message),
        //     404 => NotFound(result.Errors[0].Message),
        //     _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        // };
        
        Console.WriteLine("CreateDruzyna endpoint został wywołany. Oto dane, które otrzymał:");
        Console.WriteLine($"Nazwa: {dto.Nazwa}");
        Console.WriteLine($"IdGry: {dto.IdGry}");
        Console.WriteLine($"CzyPubliczna: {dto.CzyPubliczna}");
        Console.WriteLine($"Opis: {dto.Opis}");
        Console.WriteLine($"IdNastrojuRozgrywki: {dto.IdNastrojuRozgrywki}");
        Console.WriteLine($"IdWymaganegoJezyka: {dto.IdWymaganegoJezyka}");
        Console.WriteLine($"IdWymaganegoStopniaBieglosciJezyka: {dto.IdWymaganegoStopniaBieglosciJezyka}");
        Console.WriteLine($"Czy18Plus: {dto.Czy18Plus}");
        Console.WriteLine($"IdPlatformy: {dto.IdPlatformy}");
        Console.WriteLine($"IdRoliKapitana: {dto.IdRoliKapitana}");
        Console.WriteLine($"Liczba WymaganychStatystyk: {dto.WymaganeStatystyki?.Count}");
        if (dto.WymaganeStatystyki != null)
        {
            int statIndex = 1;
            foreach (var stat in dto.WymaganeStatystyki)
            {                Console.WriteLine($"  Statystyka {statIndex}:");
                Console.WriteLine($"    IdStatystyki: {stat.IdStatystyki}");
                Console.WriteLine($"    MinimalnaWartosc: {stat.Wartosc}");
                statIndex++;
            }
        }
        Console.WriteLine($"Liczba MiejscWDruzynie: {dto.MiejscaWDruzynie.Count}");
        int miejsceIndex = 1;
        foreach (var miejsce in dto.MiejscaWDruzynie)        {
            Console.WriteLine($"  Miejsce {miejsceIndex}:");
            Console.WriteLine($"    IdRoli: {miejsce.IdRoli}");
            miejsceIndex++;
        }
        return NoContent(); // tylko do testów
    }
}