using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Squadra.Server.Modules.Profile.DTO.KrajRegion;
using Squadra.Server.Modules.Profile.Services;

namespace Squadra.Server.Modules.Profile.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class RegionController(IRegionService regionService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Zwraca dane wszystkich istniejących w bazie regionów.")]
    [ProducesResponseType(typeof(IEnumerable<RegionDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<RegionDto>>> GetRegiony()
    {
        var result = await regionService.GetRegiony();
        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Zwraca dane regionu o podanym id.")]
    [ProducesResponseType(typeof(RegionDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<RegionDto?>> GetRegion(int id)
    {
        var result = await regionService.GetRegion(id);
        return result.StatusCode switch
        {
            400 => BadRequest(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => Ok(result.Value)
        };
    }

    [HttpGet("kraj/{id:int}")]
    [EndpointSummary("Zwraca dane wszystkich regionów kraju o podanym id.")]
    [ProducesResponseType(typeof(IEnumerable<RegionDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IEnumerable<RegionDto>>> GetRegionyKraju(int id)
    {
        var result = await regionService.GetRegionyKraju(id);
        return result.StatusCode switch
        {
            400 => BadRequest(result.Errors[0].Message),
            404 => NotFound(result.Errors[0].Message),
            _ => Ok(result.Value)
        };
    }
}