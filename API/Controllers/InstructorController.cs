using Application.Service;
using Domain.Entity;
using Domain.Exception;
using Domain.Value_object;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstructorController(AthleteService athleteService) : Controller
{
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPut("promote/{email}")]
    public async Task<IActionResult> PromoteToInstructor([FromRoute] string email)
    {
        try
        {
            EmailAddress emailAddress = new(email);
            await athleteService.PromoteToInstructorAsync(emailAddress);
            return Ok(new { message = "Athlete promoted to instructor successfully" });
        }
        catch (AthleteNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to promote athlete to instructor", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [Authorize(Roles = "Instructor")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllAthletes()
    {
        try
        {
            IEnumerable<Athlete> athletes = await athleteService.GetAllAthletesAsync();
            return Ok(athletes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve athletes", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [HttpDelete("{email}")]
    public async Task<IActionResult> DeleteAthlete([FromRoute] string email)
    {
        try
        {
            EmailAddress emailAddress = new(email);
            await athleteService.DeleteAthleteByEmailAsync(emailAddress);
            return Ok(new { message = "Athlete deleted successfully" });
        }
        catch (AthleteNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to delete athlete", details = ex.InnerException?.Message ?? ex.Message });
        }
    }
}