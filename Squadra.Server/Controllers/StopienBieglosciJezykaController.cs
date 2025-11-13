using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.DTO.JezykStopien;
using Squadra.Server.Services;

namespace Squadra.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class StopienBieglosciJezykaController(IStopienBieglosciJezykaService stopienBieglosciJezykaService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StopienBieglosciJezykaDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<StopienBieglosciJezykaDto>>> GetStopienBieglosciJezyka()
    {
        var result = await stopienBieglosciJezykaService.GetStopnieBieglosciJezyka();
        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StopienBieglosciJezykaDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<StopienBieglosciJezykaDto?>> GetStopienBieglosciJezyka(int id)
    {
        var result = await stopienBieglosciJezykaService.GetStopienBieglosciJezyka(id);
        if(result.StatusCode == 404)
            return NotFound(result.Errors[0].Message);
        return Ok(result.Value);
    }
}