using Microsoft.AspNetCore.Mvc;
using Squadra.Server.DTO.JezykStopien;
using Squadra.Server.Exceptions;
using Squadra.Server.Repositories;

namespace Squadra.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StopienBieglosciJezykaController(IStopienBieglosciJezykaRepository stopienBieglosciJezykaRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StopienBieglosciJezykaDto>>> GetStopienBieglosciJezyka()
    {
        return Ok(await stopienBieglosciJezykaRepository.GetStopnieBieglosciJezyka());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StopienBieglosciJezykaDto?>> GetStopienBieglosciJezyka(int id)
    {
        try{
            return Ok(await stopienBieglosciJezykaRepository.GetStopienBieglosciJezyka(id));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return NotFound(e.Message);
        }
    }
    
    
}