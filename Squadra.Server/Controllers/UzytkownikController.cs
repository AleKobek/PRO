using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Squadra.Server.DTO.Uzytkownik;
using Squadra.Server.Models;
using Squadra.Server.Services;

namespace Squadra.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UzytkownikController(IUzytkownikService uzytkownikService,
    UserManager<Uzytkownik> userManager) : ControllerBase
{

    // GET: api/Uzytkownik
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UzytkownikResDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<UzytkownikResDto>>> GetUzytkownicy()
    {
        var result = await uzytkownikService.GetUzytkownicy();
        return Ok(result.Value);
    }

    // GET: api/Uzytkownik/5
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UzytkownikResDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<UzytkownikResDto>> GetUzytkownikById(int id)
    {
        try
        {
            var result = await uzytkownikService.GetUzytkownik(id);
            if (result.StatusCode == 404)
                return NotFound(result.Errors[0].Message);

            return Ok(result.Value);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // PUT: api/Uzytkownik/id
    // Aktualizacja oparta na zawartości DTO (repozytorium przyjmuje UzytkownikUpdateDto).
    [HttpPut("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult> UpdateUzytkownik(int id, [FromBody] UzytkownikUpdateDto dto)
    {
        
        // User to ClaimsPrincipal, który ASP.NET Core wypełnia na podstawie cookie (tu Identity cookie)
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");

        // porównujemy id, jeżeli ktoś próbuje zedytować nie swój 
        if (uzytkownik.Id != id)
            return Forbid("Nie możesz edytować konta innego użytkownika.");
        
        var result = await uzytkownikService.UpdateUzytkownik(id, dto);
        switch (result.StatusCode)
        {
            case 204:
                return NoContent();
            case 404:
                return NotFound(result.Errors[0].Message);
            // czyli 400 lub inny syf, ale zakładam że 400
            default:
                return BadRequest(result.Errors);
        }
    }

    [HttpPut("{id:int}/haslo")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult> UpdateHaslo(int id, ZmienHasloDto dto)
    {
        // User to ClaimsPrincipal, który ASP.NET Core wypełnia na podstawie cookie (tu Identity cookie)
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");

        // porównujemy id, jeżeli ktoś próbuje zedytować nie swój 
        if (uzytkownik.Id != id)
            return Forbid("Nie możesz zmienić hasła innego użytkownika.");
        
        var result = await uzytkownikService.UpdateHaslo(id, dto.StareHaslo, dto.NoweHaslo);
        switch (result.StatusCode)
        {
            case 400:
                return BadRequest(result.Errors);
            case 404:
                return NotFound(result.Errors[0].Message);
            default:
                return NoContent();
        }
    }
    
    [HttpPut("{id:int}/haslo/admin")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult> UpdateHasloAdmin([FromRoute]int id, string stareHaslo, string noweHaslo)
    {
        
        var result = await uzytkownikService.UpdateHaslo(id, stareHaslo, noweHaslo);
        switch (result.StatusCode)
        {
            case 400:
                return BadRequest(result.Errors);
            case 404:
                return NotFound(result.Errors[0].Message);
            default:
                return NoContent();
        }
    }
    
    
    // DELETE: api/Uzytkownik/id
    [HttpDelete("{id:int}")]
    [Authorize]
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
            return Forbid("Nie możesz usunąć konta innego użytkownika.");
        
        var result = await uzytkownikService.DeleteUzytkownik(id);
        
        // mamy tylko dwie opcje, albo się udało albo nie znalazło
        if(result.StatusCode == 204)
            return NoContent();
        
        return NotFound();
    }
    
    // co minutę dostajemy od zalogowanego użytkownika, że jest online
    // nie jest to zależne od zalogowania, może być offline i zalogowany
    // znowu zrobi się online, jak otworzy stronę
    [Authorize]
    [HttpGet("ping")]
    public async Task<IActionResult> Ping()
    {
        var user = await userManager.GetUserAsync(User);
        if (user != null)
        {
            user.OstatniaAktywnosc = DateTime.Now;
            await userManager.UpdateAsync(user);
        }
        // nie mamy nic zwracać, to jest tylko po to, aby to aktualizować
        return Ok();
    }

}