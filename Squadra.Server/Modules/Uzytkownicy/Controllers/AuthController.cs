using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.Modules.Profile.Services;
using Squadra.Server.Modules.Uzytkownicy.DTO.Auth;
using Squadra.Server.Modules.Uzytkownicy.DTO.Uzytkownik;
using Squadra.Server.Modules.Uzytkownicy.Models;
using Squadra.Server.Modules.Uzytkownicy.Services;

namespace Squadra.Server.Modules.Uzytkownicy.Controllers;

// do rejestracji, logowania i wylogowywania
[ApiController]
[Route("api/[controller]")]
public class AuthController(IUzytkownikService uzytkownikService,
    IProfilService profilService,
    UserManager<Uzytkownik> userManager,
    SignInManager<Uzytkownik> signInManager) : ControllerBase
{

    [HttpPost("register")]
    [AllowAnonymous]
    [EndpointSummary("Rejestruje nowego użytkownika")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails),(int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    public async Task<IActionResult> Zarejestruj([FromBody] UzytkownikCreateDto dto)
    {
        var result = await uzytkownikService.CreateUzytkownik(dto);
        
        // jeżeli coś jest źle
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
            // Dla 409/404 itp. zwracamy odpowiedni kod z listą błędów
            case 409:
                foreach (var e in result.Errors)
                    ModelState.AddModelError(e.Field ?? string.Empty, e.Message);
                return Conflict();
            case 404:
                return NotFound(new { errors = result.Errors });
            // coś jeszcze innego
            default:
                return StatusCode(result.StatusCode, new { errors = result.Errors });
        }
    }
    
    [HttpPost("login")]
    [AllowAnonymous]
    [EndpointSummary("Loguje użytkownika")]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    // robimy robotę service, bo byłoby tylko na tę jedną funkcję
    public async Task<IActionResult> Zaloguj([FromBody] LoginRequest req)
    {
        
        // nie pozwalamy się zalogować, jeżeli już jest się zalogowanym - trzeba się najpierw wylogować
        var obecnyUzytkownik = await userManager.GetUserAsync(User);
        if(obecnyUzytkownik is not null) 
            return BadRequest("Jesteś już zalogowany. Wyloguj się, żeby zalogować na inne konto.");
        
        // Pozwalamy logować przez email lub nazwę użytkownika
        var uzytkownik = await userManager.FindByEmailAsync(req.LoginLubEmail) 
                   ?? await userManager.FindByNameAsync(req.LoginLubEmail);

        if (uzytkownik is null)
            return Unauthorized(new { message = "Nieprawidłowe dane logowania." });

        var pwCheck = await signInManager.CheckPasswordSignInAsync(uzytkownik, req.Haslo, lockoutOnFailure: true);
        if (!pwCheck.Succeeded)
            return Unauthorized(new { message = "Nieprawidłowe dane logowania." });

        await signInManager.SignInAsync(uzytkownik, isPersistent: req.ZapamietajMnie);
        
        // od razu zaznaczamy mu, że jest aktywny
        uzytkownik.OstatniaAktywnosc = DateTime.Now;
        await userManager.UpdateAsync(uzytkownik);
        
        return NoContent();
    }

    [HttpPost("logout")]
    [Authorize]
    [EndpointSummary("Wylogowuje użytkownika")]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> Wyloguj()
    {
        // nie może się wylogować, jeżeli nie jest zalogowany
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        
        await signInManager.SignOutAsync();
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    [EndpointSummary("Zwraca informacje o aktualnie zalogowanym użytkowniku")]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(AuthUserDto),(int)HttpStatusCode.OK)]
    public async Task<IActionResult> Me()
    {
        // User To obiekt typu ClaimsPrincipal, automatycznie wypełniany przez ASP.NET Core na podstawie cookie.
        // Zawiera informacje (claims) o zalogowanym użytkowniku, np. jego UserId, UserName, role itp.
        
        // ASP.NET Identity używa tego, żeby znaleźć w bazie obiekt Uzytkownik odpowiadający zalogowanej osobie
        // (na podstawie claimu NameIdentifier w cookie).
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null) return Unauthorized();

        var roles = (await userManager.GetRolesAsync(uzytkownik)).ToArray();
        var result = await profilService.GetProfil(uzytkownik.Id);
        var profil = result.Value;
        var awatar = profil?.Awatar;
        return Ok(new AuthUserDto(uzytkownik.Id, uzytkownik.UserName!, uzytkownik.Email!, roles, awatar));
    }

}