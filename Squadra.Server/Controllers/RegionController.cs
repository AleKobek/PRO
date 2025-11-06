using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.DTO.KrajRegion;
using Squadra.Server.Exceptions;
using Squadra.Server.Services;

namespace Squadra.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class RegionController(IRegionService regionService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RegionDto>>> GetRegiony()
    {
        var result = await regionService.GetRegiony();
        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RegionDto?>> GetRegion(int id)
    {
        var result = await regionService.GetRegion(id);
        if (result.StatusCode == 404) return NotFound(result.Errors[0].Message);
        return Ok(result.Value);
    }

    [HttpGet("kraj/{id}")]
    public async Task<IActionResult> GetRegionyKraju(int id)
    {
        var result = await regionService.GetRegionyKraju(id);
        if(result.StatusCode == 404) return NotFound(result.Errors[0].Message);;
        return Ok(result.Value);
    }
}