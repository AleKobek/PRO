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
    public async Task<ActionResult<IEnumerable<KrajDto>>> GetKraje()
    {
        var result = await krajService.GetKraje();
        return Ok(result.Value);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<KrajDto?>> GetKraj(int id)
    {
        var result = await krajService.GetKraj(id);
        if(result.StatusCode == 404)
            return NotFound(result.Errors[0].Message);;
        return Ok(result.Value);
    }
}