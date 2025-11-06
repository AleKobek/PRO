using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.DTO.Profil;
using Squadra.Server.DTO.Status;
using Squadra.Server.Exceptions;
using Squadra.Server.Models;
using Squadra.Server.Services;

namespace Squadra.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]

public class ProfilController(IProfilService profilService,
    UserManager<Uzytkownik> userManager) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<ProfilGetResDto>> GetProfil(int id)
    {
        var result = await profilService.GetProfil(id);
        if(result.StatusCode == 404) return NotFound(result.Errors[0].Message);
        return Ok(result.Value);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProfilGetResDto>> UpdateProfil(int id, [FromBody] ProfilUpdateDto profil)
    {
        // User to ClaimsPrincipal, który ASP.NET Core wypełnia na podstawie cookie (tu Identity cookie)
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");

        // porównujemy id, jeżeli ktoś próbuje zedytować nie swój 
        Console.WriteLine("Id uzytkownika: " + uzytkownik.Id + ", id profilu: " + id);
        if (uzytkownik.Id != id)
            return Forbid("Nie możesz edytować profilu innego użytkownika.");
        
        var result = await profilService.UpdateProfil(id, profil);
        switch (result.StatusCode)
        {
            case 404:
                return NotFound(result.Errors[0].Message);
            case 400:
                return BadRequest(result.Errors);
            default:
                ;
                return Ok(result.Value);
        }
    }

    // w reszcie tak samo i not found tak samo i na froncie dostosować
    [HttpGet("{id}/status/baza")]
    public async Task<ActionResult<StatusDto>> GetStatusZBazyProfilu(int id)
    {
        var result = await profilService.GetStatusZBazyProfilu(id);
        if(result.StatusCode == 404) return NotFound(result.Errors[0].Message);
        return Ok(result.Value);
    }

    [HttpGet("{id}/status/wyswietlenie")]
    public async Task<ActionResult<StatusDto>> GetStatusDoWyswietleniaProfilu(int id)
    {
        var result = await profilService.GetStatusDoWyswietleniaProfilu(id);
        if(result.StatusCode == 404) return NotFound(result.Errors[0].Message);
        return Ok(result.Value);
    }

    [HttpPut("{id}/status")]
    private async Task<ActionResult<ProfilGetResDto>> UpdateStatus(int id, [FromBody] int idStatus)
    {
        
        // // porównujemy id, jeżeli ktoś próbuje zedytować nie swój 
        // if (uzytkownik.Id != id)
        //     return Forbid("Nie możesz edytować profilu innego użytkownika.");
        var result = await profilService.GetProfil(id);
        switch (result.StatusCode)
        {
            case 404:
                return NotFound(result.Errors[0].Message);
            case 400:
                return BadRequest(result.Errors[0].Message);
            default:
                ;
                return Ok(result.Value);
        }
    }
}