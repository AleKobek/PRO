using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.DTO.Status;
using Squadra.Server.Exceptions;
using Squadra.Server.Services;

namespace Squadra.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class StatusController(IStatusService statusService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ICollection<StatusDto>>> GetStatusy()
    {
        var result = await statusService.GetStatusy();
        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StatusDto?>> GetStatus(int id)
    {
        
        var result = await statusService.GetStatus(id);
        if (result.StatusCode == 404)
            return NotFound(result.Errors[0].Message);
        return Ok(result.Value);
    }
}