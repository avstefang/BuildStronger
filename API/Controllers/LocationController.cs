using Application.Dto;
using Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LocationController(LocationService locationService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllLocations()
    {
        try
        {
            IEnumerable<GetLocationDto> locations = await locationService.GetAllLocationsAsync();
            return Ok(locations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve locations", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetLocationById([FromRoute] Guid id)
    {
        try
        {
            GetLocationDto? location = await locationService.GetLocationByIdAsync(id);
            if (location == null) return NotFound(new { error = $"Location {id} not found" });
            return Ok(location);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve location", details = ex.InnerException?.Message ?? ex.Message });
        }
    }
}
