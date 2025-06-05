using CW_10.DTOs;
using CW_10.Exceptions;
using CW_10.Services;
using Microsoft.AspNetCore.Mvc;

namespace CW_10.Controllers;


[ApiController]
[Route("api")]
public class TripController (IDbService service) : ControllerBase
{

    //http://localhost:5012/api/trips?page=2&pageSize=2
    [HttpGet]
    [Route("trips")]
    public async Task<IActionResult> GetAllTripsWithDetailsOnPage([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1)
        {
            return BadRequest("Page number must be greater than 0.");
        }
        
        if (pageSize < 1)
        {
            return BadRequest("Page size must be greater than 0.");
        }

        return Ok(await service.GetAllTripsWithDetailsOnPage( page, pageSize));
    }
    
    

    //http://localhost:5012/api/clients/4
    [HttpDelete]
    [Route("clients/{id}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        try
        {
            await service.DeleteClient(id);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
    }
    
    

    [HttpPost]
    [Route("trips/{idTrip}/clients")]
    public async Task<IActionResult> CreateClientWithTrips(int idTrip,[FromBody] ClientWithTripsCreateDto a)
    {
        try
        {
            await service.CreateClientWithTrips(idTrip, a);
            return Created();

        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
        catch (Exception)
        {
            return Conflict("key duplication");
        }
        
    }
    
}