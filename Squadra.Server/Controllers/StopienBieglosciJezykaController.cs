using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.DTO.JezykStopien;
using Squadra.Server.Exceptions;
using Squadra.Server.Repositories;
using Squadra.Server.Services;

namespace Squadra.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class StopienBieglosciJezykaController(IStopienBieglosciJezykaService stopienBieglosciJezykaService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StopienBieglosciJezykaDto>>> GetStopienBieglosciJezyka()
    {
        var result = await stopienBieglosciJezykaService.GetStopnieBieglosciJezyka();
        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StopienBieglosciJezykaDto?>> GetStopienBieglosciJezyka(int id)
    {
        var result = await stopienBieglosciJezykaService.GetStopienBieglosciJezyka(id);
        if(result.StatusCode == 404)
            return NotFound(result.Errors[0].Message);
        return Ok(result.Value);
    }
    
    // pozmieniać też w reszcie!
}