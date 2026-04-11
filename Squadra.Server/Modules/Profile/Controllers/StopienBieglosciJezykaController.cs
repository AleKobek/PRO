using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.Modules.Profile.DTO.JezykStopien;
using Squadra.Server.Modules.Profile.Services;

namespace Squadra.Server.Modules.Profile.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class StopienBieglosciJezykaController(IStopienBieglosciJezykaService stopienBieglosciJezykaService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Zwraca dane wszystich stopni biegłości języka w bazie.")]
    [ProducesResponseType(typeof(IEnumerable<StopienBieglosciJezykaDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<StopienBieglosciJezykaDto>>> GetStopienBieglosciJezyka()
    {
        var result = await stopienBieglosciJezykaService.GetStopnieBieglosciJezyka();
        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Zwraca dane stopnia biegłości języka o podanym id.")]
    [ProducesResponseType(typeof(StopienBieglosciJezykaDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<StopienBieglosciJezykaDto?>> GetStopienBieglosciJezyka(int id)
    {
        var result = await stopienBieglosciJezykaService.GetStopienBieglosciJezyka(id);
        return result.StatusCode switch
        {
            400 => BadRequest(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => Ok(result.Value)
        };
    }
}