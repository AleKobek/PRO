using Microsoft.AspNetCore.Mvc;
using Squadra.Exceptions;
using Squadra.Services;

namespace Squadra;

[Route("api/[controller]")]
[ApiController]
public class KrajController(IKrajService krajService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<KrajDto>>> GetKraje()
    {
        return Ok(await krajService.GetKraje());
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<KrajDto?>> GetKraj(int id)
    {
        try{
            return Ok(await krajService.GetKraj(id));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return NotFound(e.Message);
        }
    }
}