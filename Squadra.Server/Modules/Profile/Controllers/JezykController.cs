using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Profile.Services;

namespace Squadra.Server.Modules.Profile.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class JezykController(IJezykService jezykService) : ControllerBase
{

    [HttpGet]
    [EndpointSummary("Zwraca dane wszystkich języków w bazie.")]
    [ProducesResponseType(typeof(IEnumerable<JezykDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<JezykDto>>> GetJezyki()
    {
        var result = await jezykService.GetJezyki();
        return Ok(result.Value);
    }
    
    [HttpGet("{id:int}")]
    [EndpointSummary("Zwraca dane języka o podanym id.")]   
    [ProducesResponseType(typeof(JezykDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<JezykDto?>> GetJezyk(int id)
    {
        var result = await jezykService.GetJezyk(id);
        return result.StatusCode switch
        {
            400 => BadRequest(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => Ok(result.Value)
        };
    }

    [HttpGet("profil/{id:int}")]
    [EndpointSummary("Zwraca dane wszystkich języków profilu o podanym id, wraz z ich stopniami biegłości.")]
    [ProducesResponseType(typeof(IEnumerable<JezykOrazStopienDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IEnumerable<JezykOrazStopienDto>>> GetJezykiProfilu(int id)
    {
        var result = await jezykService.GetJezykiProfilu(id);
        return result.StatusCode switch
        {
            400 => BadRequest(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => Ok(result.Value)
        };
    }

}