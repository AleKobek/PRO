﻿using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.DTO.Powiadomienie;
using Squadra.Server.DTO.Profil;
using Squadra.Server.Models;
using Squadra.Server.Modules.Znajomi.DTO;
using Squadra.Server.Services;

namespace Squadra.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ZnajomiController(IZnajomiService znajomiService, 
    IPowiadomienieService powiadomienieService, 
    UserManager<Uzytkownik> userManager,
    IProfilService profilService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Zwraca listę znajomych danego użytkownika")]
    [EndpointDescription("Zawiera tylko część informacji o znajomym potrzebną do wyświetlenia na liście znajomych (id, pseudonim, url zdjęcia profilowego, data ostatniego otwarcia czatu), bez szczegółów profilu.")]
    [ProducesResponseType(typeof(IEnumerable<ZnajomyDoListyDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IEnumerable<ZnajomyDoListyDto>>> GetZnajomiDoListy()
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        var result = await znajomiService.GetZnajomiDoListyUzytkownika(uzytkownik.Id);
        if (result.StatusCode == 404) return NotFound(result.Errors[0].Message);
        return Ok(result.Value);
    }
    
    [HttpGet("czyZnajomosc/{idZnajomego:int}")]
    [EndpointSummary("Zwraca, czy jest znajomość pomiędzy zalogowanym użytkownikiem a użytkownikiem o podanym id")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<bool>> CzyJestZnajomosc(int idZnajomego)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await znajomiService.CzyJestZnajomosc(uzytkownik.Id, idZnajomego);
        
        if(result.StatusCode == 404) return NotFound(result.Errors[0].Message);
        
        return Ok(result.Value);
    }
    
    [HttpDelete("{idUsuwanego:int}")]
    [EndpointSummary("Usuwa znajomość pomiędzy zalogowanym użytkownikiem a użytkownikiem o podanym id")]
    [EndpointDescription("Wraz z tym jest usuwana ich historia wiadomości")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<bool>> DeleteZnajomego(int idUsuwanego)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await znajomiService.DeleteZnajomosc(uzytkownik.Id, idUsuwanego);
        
        if(result.StatusCode == 404) return NotFound(result.Errors[0].Message);
        
        // wyszukujemy nasz profil, aby dołączyć pseudonim do powiadomienia, które wyślemy do tego, który został usunięty
        var wynikWyszukiwaniaProfilu = await profilService.GetProfil(uzytkownik.Id);
        if (wynikWyszukiwaniaProfilu.StatusCode == 404 || wynikWyszukiwaniaProfilu.Value == null)
            return NotFound(wynikWyszukiwaniaProfilu.Errors[0].Message);
        
        var resultPowiadomienia = await powiadomienieService.CreatePowiadomienie(new PowiadomienieCreateDto(
            5,
            idUsuwanego, // wysyłamy to temu, który został usunięty
            uzytkownik.Id,
            wynikWyszukiwaniaProfilu.Value.Pseudonim,
            null
        ));

        return resultPowiadomienia.StatusCode switch
        {
            204 => NoContent(),
            404 => NotFound(resultPowiadomienia.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
    
    [HttpPut("{idZnajomego:int}")]
    [EndpointSummary("Aktualizuje datę ostatniego otwarcia czatu zalogowanego użytkownika z danym znajomym")]
    [EndpointDescription("Służy do tego, żeby wiedzieć, czy są jakieś nowe wiadomości od tego znajomego. Jeśli data ostatniego otwarcia czatu jest starsza niż data ostatniej wiadomości od tego znajomego, to znaczy, że są nowe wiadomości.")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<bool>> ZaktualizujOstatnieOtwarcieCzatu(int idZnajomego)
    {
        var uzytkownik = await userManager.GetUserAsync(User);
        if (uzytkownik is null)
            return Unauthorized("Nie jesteś zalogowany.");
        
        var result = await znajomiService.ZaktualizujOstatnieOtwarcieCzatu(uzytkownik.Id, idZnajomego);
        
        if(result.StatusCode == 404) return NotFound(result.Errors[0].Message);
        
        return NoContent();
    }
}