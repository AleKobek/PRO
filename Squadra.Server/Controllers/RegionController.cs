using Microsoft.AspNetCore.Mvc;
using Squadra.Exceptions;
using Squadra.Services;

namespace Squadra;


[Route("api/[controller]")]
[ApiController]
public class RegionController(IRegionService regionService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RegionDto>>> GetRegiony()
    {
        return Ok(await regionService.GetRegiony());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RegionDto?>> GetRegion(int id)
    {
        try
        {
            return Ok(await regionService.GetRegion(id));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpGet("kraj/{id}")]
    public async Task<IActionResult> GetRegionyKraju(int id)
    {
        try{
            return Ok(await regionService.GetRegionyKraju(id));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return NotFound(e.Message);
        }
    }
}