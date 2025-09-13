using Microsoft.AspNetCore.Mvc;
using Squadra.Exceptions;
using Squadra.Services;

namespace Squadra;

[Route("api/[controller]")]
[ApiController]
public class JezykController(IJezykService jezykService) : ControllerBase
{


    [HttpGet]
    public async Task<ActionResult<IEnumerable<JezykDto>>> GetJezyki()
    {
        return Ok(await jezykService.GetJezyki());
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<JezykDto?>> GetJezyk(int id)
    {
        try
        {
            return Ok(await jezykService.GetJezyk(id));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpGet("profil/{id}")]
    public async Task<ActionResult<ICollection<JezykOrazStopienDto>>> GetJezykiProfilu(int id)
    {
        try
        {
            return Ok(await jezykService.GetJezykiProfilu(id));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return NotFound(e.Message);
        }
    }
    

}