using Application.Service;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LocationController(LocationService locationService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllLocations()
    {
        try
        {
            IEnumerable<Location> locations = await locationService.GetAllLocationsAsync();
            return Ok(locations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve locations", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetLocationById([FromRoute] Guid id)
    {
        try
        {
            Location? location = await locationService.GetLocationByIdAsync(id);
            if (location == null) return NotFound(new { error = $"Location {id} not found" });
            return Ok(location);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve location", details = ex.InnerException?.Message ?? ex.Message });
        }
    }
}
