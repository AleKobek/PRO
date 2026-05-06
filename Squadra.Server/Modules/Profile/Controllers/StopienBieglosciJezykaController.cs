using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.Modules.Profile.Models;
using Squadra.Server.Modules.Profile.Services;

namespace Squadra.Server.Modules.Profile.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class StopienBieglosciJezykaController(IStopienBieglosciJezykaService stopienBieglosciJezykaService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Zwraca dane wszystich stopni biegłości języka w bazie.")]
    [ProducesResponseType(typeof(IEnumerable<StopienBieglosciJezyka>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<StopienBieglosciJezyka>>> GetStopienBieglosciJezyka()
    {
        var result = await stopienBieglosciJezykaService.GetStopnieBieglosciJezyka();
        return result.StatusCode == 200
            ? Ok(result.Value)
            : StatusCode(result.StatusCode, new { errors = result.Errors });
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Zwraca dane stopnia biegłości języka o podanym id.")]
    [ProducesResponseType(typeof(StopienBieglosciJezyka), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<StopienBieglosciJezyka?>> GetStopienBieglosciJezyka(int id)
    {
        var result = await stopienBieglosciJezykaService.GetStopienBieglosciJezyka(id);
        return result.StatusCode switch
        {
            200 => Ok(result.Value),
            400 => BadRequest(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => StatusCode(result.StatusCode, new { errors = result.Errors })
        };
    }
}