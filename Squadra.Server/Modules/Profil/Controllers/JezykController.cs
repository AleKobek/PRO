using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.DTO.JezykStopien;
using Squadra.Server.Services;

namespace Squadra.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class JezykController(IJezykService jezykService) : ControllerBase
{


    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<JezykDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<JezykDto>>> GetJezyki()
    {
        var result = await jezykService.GetJezyki();
        return Ok(result.Value);
    }
    
    [HttpGet("{id:int}")]   
    [ProducesResponseType(typeof(JezykDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<JezykDto?>> GetJezyk(int id)
    {
        var result = await jezykService.GetJezyk(id);
        if(result.StatusCode == 404)
            return NotFound(result.Errors[0].Message);;
        return Ok(result.Value);
    }

    [HttpGet("profil/{id:int}")]
    [ProducesResponseType(typeof(IEnumerable<JezykOrazStopienDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IEnumerable<JezykOrazStopienDto>>> GetJezykiProfilu(int id)
    {
        var result = await jezykService.GetJezykiProfilu(id);
        if(result.StatusCode == 404)
            return NotFound(result.Errors[0].Message);;
        return Ok(result.Value);
    }

}