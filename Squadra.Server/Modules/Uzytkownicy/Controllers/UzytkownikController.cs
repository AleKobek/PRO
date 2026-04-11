using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Squadra.Server.Modules.Uzytkownicy.DTO.Uzytkownik;
using Squadra.Server.Modules.Uzytkownicy.Models;
using Squadra.Server.Modules.Uzytkownicy.Services;

namespace Squadra.Server.Modules.Uzytkownicy.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UzytkownikController(IUzytkownikService uzytkownikService,
    UserManager<Uzytkownik> userManager) : ControllerBase
{
    [HttpGet("wszyscy")]
    [Authorize(Roles = "Admin")]
    [EndpointSummary("Zwraca dane wszystkich użtykowników w bazie (tylko dla admina)")]
    [ProducesResponseType(typeof(IEnumerable<UzytkownikResDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<UzytkownikResDto>>> GetUzytkownicy()
    {
        var result = await uzytkownikService.GetUzytkownicy();
        return result.StatusCode == 200
            ? Ok(result.Value)
            : StatusCode(result.StatusCode, new { errors = result.Errors });
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    [EndpointSummary("Zwraca dane użytkownika o podanym id (tylko dla admina)")]
    [ProducesResponseType(typeof(UzytkownikResDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<UzytkownikResDto>> GetUzytkownikById(int id)
    {
        try
        {
            var result = await uzytkownikService.GetUzytkownik(id);
            return result.StatusCode switch
            {
                200 => Ok(result.Value),
                400 => BadRequest(result.Errors[0].Message),
                404 => NotFound(result.Errors[0].Message),
                _ => StatusCode(result.StatusCode, new { errors = result.Errors })
            };
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
    
    [HttpGet]
    [EndpointSummary("Zwraca dane zalogowanego użytkownika")]
    [ProducesResponseType(typeof(UzytkownikResDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<UzytkownikResDto>> GetUzytkownik()
    {
        try
        {
            var uzytkownik = await userManager.GetUserAsync(User);
            if (uzytkownik is null)
                return Unauthorized("Nie jesteś zalogowany.");
            
            var result = await uzytkownikService.GetUzytkownik(uzytkownik.Id);
            if (result.StatusCode == 404)
                return NotFound(result.Errors[0].Message);

            return result.StatusCode == 200
                ? Ok(result.Value)
                : StatusCode(result.StatusCode, new { errors = result.Errors });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [EndpointSummary("Aktualizuje dane (poza hasłem) użytkownika o podanym id (tylko dla admina)")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> UpdateUzytkownik(int id, [FromBody] UzytkownikUpdateDto dto)
    {
        
        // User to ClaimsPrincipal, który ASP.NET Core wypełnia na podstawie cookie (tu Identity cookie)
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await uzytkownikService.UpdateUzytkownik(id, dto);
        switch (result.StatusCode)
        {
            case 204:
                return NoContent();
            case 400:
                foreach (var e in result.Errors)
                    ModelState.AddModelError(e.Field ?? string.Empty, e.Message);
                return ValidationProblem();
            case 404:
                return NotFound(result.Errors[0].Message);
            default:
                return StatusCode(result.StatusCode, new { errors = result.Errors });
        }
    }
    
    [HttpPut]
    [EndpointSummary("Aktualizuje dane (poza hasłem) zalogowanego użytkownika")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> UpdateUzytkownik([FromBody] UzytkownikUpdateDto dto)
    {
        
        // User to ClaimsPrincipal, który ASP.NET Core wypełnia na podstawie cookie (tu Identity cookie)
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await uzytkownikService.UpdateUzytkownik(uzytkownik.Id, dto);
        switch (result.StatusCode)
        {
            case 204:
                return NoContent();
            case 400:
                foreach (var e in result.Errors)
                    ModelState.AddModelError(e.Field ?? string.Empty, e.Message);
                return ValidationProblem();
            case 404:
                return NotFound(result.Errors[0].Message);
            default:
                return StatusCode(result.StatusCode, new { errors = result.Errors });
        }
    }

    [HttpPut("haslo")]
    [EndpointSummary("Aktualizuje hasło zalogowanego użytkownika")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> UpdateHaslo(ZmienHasloDto dto)
    {
        // User to ClaimsPrincipal, który ASP.NET Core wypełnia na podstawie cookie (tu Identity cookie)
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await uzytkownikService.UpdateHaslo(uzytkownik.Id, dto.StareHaslo, dto.NoweHaslo);
        return result.StatusCode switch
        {
            204 => NoContent(),
            400 => BadRequest(result.Errors),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
    
    [HttpPut("{id:int}/haslo")]
    [Authorize(Roles = "Admin")]
    [EndpointSummary("Zmienia hasło użytkownika o podanym id (tylko dla admina)")]
    [EndpointDescription("Nie sprawdza starego hasła, ponieważ admin może zmieniać hasło bez jego znajomości.")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> UpdateHasloAdmin([FromRoute]int id, string stareHaslo, string noweHaslo)
    {
        var result = await uzytkownikService.UpdateHaslo(id, stareHaslo, noweHaslo);
        return result.StatusCode switch
        {
            204 => NoContent(),
            400 => BadRequest(result.Errors),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
    
    
    [HttpDelete("{id:int}")]
    [Authorize]
    [EndpointSummary("Usuwa użytkownika o podanym id")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<IActionResult> DeleteUzytkownik(int id)
    {
        
        // User to ClaimsPrincipal, który ASP.NET Core wypełnia na podstawie cookie (tu Identity cookie)
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");

        // porównujemy id, jeżeli ktoś próbuje zedytować nie swój 
        if (uzytkownik.Id != id)
            return StatusCode(StatusCodes.Status403Forbidden, "Nie możesz usunąć konta innego użytkownika.");
        
        var result = await uzytkownikService.DeleteUzytkownik(id);

        return result.StatusCode switch
        {
            204 => NoContent(),
            400 => BadRequest(result.Errors[0].Message),
            404 => NotFound(),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
    
    [Authorize]
    [HttpGet("ping")]
    [EndpointSummary("Sygnał od klienta, że użytkownik jest online")]
    [EndpointDescription("Co minutę dostajemy od zalogowanego użytkownika, że jest online. Nie jest to zależne od zalogowania, może być offline i zalogowany. Znowu zrobi się online, jak otworzy stronę.")]
    public async Task<IActionResult> Ping()
     {
         var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier); // Pobieramy ID z claimów
         if (int.TryParse(userIdString, out var userId))
         {
             var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
             if (user != null)
             {
                 user.OstatniaAktywnosc = DateTime.Now;
                 await userManager.UpdateAsync(user);
             }
         }
         return Ok();
     }


}