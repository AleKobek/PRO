using Microsoft.AspNetCore.Mvc;
using Squadra.Server.DTO.Status;
using Squadra.Server.Exceptions;
using Squadra.Server.Services;

namespace Squadra.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatusController(IStatusService statusService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ICollection<StatusDto>>> GetStatusy()
    {
       return Ok(await statusService.GetStatusy());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StatusDto?>> GetStatus(int id)
    {
        try{
            return Ok(await statusService.GetStatus(id));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return NotFound(e.Message);
        }
    }
}