using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.DTO.JezykStopien;
using Squadra.Server.Exceptions;
using Squadra.Server.Services;

namespace Squadra.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class JezykController(IJezykService jezykService) : ControllerBase
{


    [HttpGet]
    public async Task<ActionResult<IEnumerable<JezykDto>>> GetJezyki()
    {
        var result = await jezykService.GetJezyki();
        return Ok(result.Value);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<JezykDto?>> GetJezyk(int id)
    {
        var result = await jezykService.GetJezyk(id);
        if(result.StatusCode == 404)
            return NotFound(result.Errors[0].Message);;
        return Ok(result.Value);
    }

    [HttpGet("profil/{id}")]
    public async Task<ActionResult<ICollection<JezykOrazStopienDto>>> GetJezykiProfilu(int id)
    {
        var result = await jezykService.GetJezykiProfilu(id);
        if(result.StatusCode == 404)
            return NotFound(result.Errors[0].Message);;
        return Ok(result.Value);
    }

}