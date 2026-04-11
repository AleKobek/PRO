using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.Modules.Profile.DTO.Status;
using Squadra.Server.Modules.Profile.Services;

namespace Squadra.Server.Modules.Profile.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class StatusController(IStatusService statusService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Zwraca dane wszystkich istniejących statusów.")]
    [ProducesResponseType(typeof(IEnumerable<StatusDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<StatusDto>>> GetStatusy()
    {
        var result = await statusService.GetStatusy();
        return result.StatusCode == 200
            ? Ok(result.Value)
            : StatusCode(result.StatusCode, new { errors = result.Errors });
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Zwraca dane statusu o podanym id.")]
    [ProducesResponseType(typeof(StatusDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<StatusDto?>> GetStatus(int id)
    {
        
        var result = await statusService.GetStatus(id);
        if (result.StatusCode == 404)
            return NotFound(result.Errors[0].Message);
        return result.StatusCode == 200
            ? Ok(result.Value)
            : StatusCode(result.StatusCode, new { errors = result.Errors });
    }
}