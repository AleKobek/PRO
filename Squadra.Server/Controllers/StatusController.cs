using Microsoft.AspNetCore.Mvc;
using Squadra.Exceptions;
using Squadra.Services;

namespace Squadra;

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