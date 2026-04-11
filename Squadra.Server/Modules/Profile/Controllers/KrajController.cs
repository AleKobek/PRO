using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.Modules.Profile.DTO.KrajRegion;
using Squadra.Server.Modules.Profile.Services;

namespace Squadra.Server.Modules.Profile.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class KrajController(IKrajService krajService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Zwraca dane wszystkich krajów w bazie.")]
    [ProducesResponseType(typeof(IEnumerable<KrajDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<KrajDto>>> GetKraje()
    {
        var result = await krajService.GetKraje();
        return result.StatusCode == 200
            ? Ok(result.Value)
            : StatusCode(result.StatusCode, new { errors = result.Errors });
    }
    
    [HttpGet("{id:int}")]
    [EndpointSummary("Zwraca dane kraju o podanym id.")]
    [ProducesResponseType(typeof(KrajDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<KrajDto?>> GetKraj(int id)
    {
        var result = await krajService.GetKraj(id);
        return result.StatusCode switch
        {
            400 => BadRequest(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => Ok(result.Value)
        };
    }
}