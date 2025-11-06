using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Squadra.Server.DTO.Uzytkownik;
using Squadra.Server.Services;

namespace Squadra.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UzytkownikController(IUzytkownikService uzytkownikService) : ControllerBase
{

    // GET: api/Uzytkownik
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ICollection<UzytkownikResDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ICollection<UzytkownikResDto>>> GetUzytkownicy()
    {
        var result = await uzytkownikService.GetUzytkownicy();
        return Ok(result.Value);
    }

    // GET: api/Uzytkownik/5
    [HttpGet("{id:int}")]
    [Authorize]
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

    // tylko dla admina
    // POST: api/Uzytkownik
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UzytkownikResDto), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    public async Task<ActionResult<UzytkownikResDto>> CreateUzytkownik([FromBody] UzytkownikCreateDto dto)
    {
        var created = await uzytkownikService.CreateUzytkownik(dto);
        
        // tutaj też sprawdzamy, czy są błędy?

        // Zwracamy 201 Created. Jeśli UzytkownikResDto ma Id, rozważ użycie CreatedAtAction z routem do GetUzytkownikById.
        return Created(string.Empty, created);
    }

    // // PUT: api/Uzytkownik
    // // Aktualizacja oparta na zawartości DTO (repozytorium przyjmuje UzytkownikUpdateDto).
    // [HttpPut]
    // [Authorize]
    // [ProducesResponseType(typeof(UzytkownikResDto), (int)HttpStatusCode.OK)]
    // [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    // [ProducesResponseType((int)HttpStatusCode.NotFound)]
    // public async Task<ActionResult<UzytkownikResDto>> UpdateUzytkownik([FromBody] UzytkownikUpdateDto dto)
    // {
    //     if (!ModelState.IsValid)
    //         return ValidationProblem(ModelState);
    //
    //     try
    //     {
    //         var updated = await uzytkownikService.UpdateUzytkownik(dto);
    //         return Ok(updated);
    //     }
    //     catch (KeyNotFoundException)
    //     {
    //         return NotFound();
    //     }
    // }
    //
    // // DELETE: api/Uzytkownik/5
    // [HttpDelete("{id:int}")]
    // [Authorize]
    // [ProducesResponseType((int)HttpStatusCode.NoContent)]
    // [ProducesResponseType((int)HttpStatusCode.NotFound)]
    // public async Task<IActionResult> DeleteUzytkownik(int id)
    // {
    //     try
    //     {
    //         await uzytkownikService.DeleteUzytkownik(id);
    //         return NoContent();
    //     }
    //     catch (KeyNotFoundException)
    //     {
    //         return NotFound();
    //     }
    // }

}