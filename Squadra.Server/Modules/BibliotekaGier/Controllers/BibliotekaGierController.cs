using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.Modules.BibliotekaGier.DTO;
using Squadra.Server.Modules.BibliotekaGier.Services;
using Squadra.Server.Modules.Uzytkownicy.Models;

namespace Squadra.Server.Modules.BibliotekaGier.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BibliotekaGierController (
        IBibliotekaGierService bibliotekaGierService,
        UserManager<Uzytkownik> userManager) : ControllerBase
{ 
        [HttpGet("{idUzytkownika:int}")]
        [EndpointSummary("Zwraca dane gier znajdujących się w bibliotece użytkownika o podanym id")]
        [ProducesResponseType(typeof(IEnumerable<GraWBiblioteceDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<GraWBiblioteceDTO>>> GetGryWBiblioteceUzytkownika(int idUzytkownika)
        {
                var uzytkownik = await userManager.GetUserAsync(User);
                if (uzytkownik is null)
                        return Unauthorized("Nie jesteś zalogowany.");
                var result = await bibliotekaGierService.PodajGryWBiblioteceUzytkownika(idUzytkownika);
                return result.StatusCode switch
                {
                        200 => Ok(result.Value),
                        400 => BadRequest(result.Errors[0].Message),
                        404 => NotFound(result.Errors[0].Message),
                        _ => StatusCode(result.StatusCode, new { errors = result.Errors })
                };
        }
}