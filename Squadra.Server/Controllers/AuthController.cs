using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.DTO.Auth;
using Squadra.Server.DTO.Uzytkownik;
using Squadra.Server.Models;
using Squadra.Server.Services;

namespace Squadra.Server.Controllers;

// do rejestracji, logowania i wylogowywania
[ApiController]
[Route("api/[controller]")]
[method: ActivatorUtilitiesConstructor]
public class AuthController(IUzytkownikService uzytkownikService,
    UserManager<Uzytkownik> userManager,
    SignInManager<Uzytkownik> signInManager) : ControllerBase
{

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UzytkownikResDto), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails),(int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    public async Task<ActionResult<UzytkownikResDto>> Zarejestruj([FromBody] UzytkownikCreateDto dto)
    {
        var result = await uzytkownikService.CreateUzytkownik(dto);
        
        // jeżeli wszystko git: zwracamy nowo utworzonego użytkownika ze statusem
        if (result.Succeeded) return StatusCode(result.StatusCode, result.Value);
        
        // jeżeli coś jest źle
        switch (result.StatusCode)
        {
            // Dla 400 u nas zwracamy ValidationProblem, ponieważ błędy dotyczą konkretnych pól
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
            default:
                return StatusCode(result.StatusCode, new { errors = result.Errors });
        }
    }
    
    [HttpPost("login")]
    [AllowAnonymous]
    // robimy robotę service, bo byłoby tylko na tę jedną funkcję
    public async Task<IActionResult> Zaloguj([FromBody] LoginRequest req)
    {
        // Pozwalamy logować przez email lub nazwę użytkownika
        Console.WriteLine("Login lub email: " + req.LoginLubEmail);
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

        var roles = (await userManager.GetRolesAsync(uzytkownik)).ToArray();
        return Ok(new AuthUserDto(uzytkownik.Id, uzytkownik.UserName!, uzytkownik.Email!, roles));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Wyloguj()
    {
        await signInManager.SignOutAsync();
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        // User To obiekt typu ClaimsPrincipal, automatycznie wypełniany przez ASP.NET Core na podstawie cookie.
        // Zawiera informacje (claims) o zalogowanym użytkowniku, np. jego UserId, UserName, role itp.
        
        // ASP.NET Identity używa tego, żeby znaleźć w bazie obiekt Uzytkownik odpowiadający zalogowanej osobie
        // (na podstawie claimu NameIdentifier w cookie).
        var user = await userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        var roles = (await userManager.GetRolesAsync(user)).ToArray();
        return Ok(new AuthUserDto(user.Id, user.UserName!, user.Email!, roles));
    }

}