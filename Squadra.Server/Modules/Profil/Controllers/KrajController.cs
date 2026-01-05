using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.DTO.KrajRegion;
using Squadra.Server.Exceptions;
using Squadra.Server.Services;

namespace Squadra.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class KrajController(IKrajService krajService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<KrajDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<KrajDto>>> GetKraje()
    {
        var result = await krajService.GetKraje();
        return Ok(result.Value);
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(KrajDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<KrajDto?>> GetKraj(int id)
    {
        var result = await krajService.GetKraj(id);
        if(result.StatusCode == 404)
            return NotFound(result.Errors[0].Message);;
        return Ok(result.Value);
    }
}