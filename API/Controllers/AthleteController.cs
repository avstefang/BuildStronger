using Application.Dto;
using Application.Interface;
using Application.Service;
using Domain.Exception;
using Domain.Value_object;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AthleteController(AthleteService athleteService) : ControllerBase
{
    /// <summary>
    /// Get all athletes (instructor/administrator only)
    /// </summary>
    [Authorize(Roles = "Instructor,Administrator")]
    [HttpGet]
    public async Task<IActionResult> GetAllAthletes()
    {
        try
        {
            IEnumerable<GetAthleteDto> athletes = await athleteService.GetAllAthletesAsync();
            return Ok(athletes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve athletes", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    /// <summary>
    /// Promote an athlete to instructor (instructor/administrator only)
    /// </summary>
    [Authorize(Roles = "Instructor,Administrator")]
    [HttpPut("{email}/promote")]
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

    /// <summary>
    /// Get athlete by ID (admin or authenticated user only)
    /// </summary>
    [Authorize]
    [HttpGet("{email}")]
    public async Task<IActionResult> GetAthleteByEmail([FromRoute] string email)
    {
        try
        {
            EmailAddress emailAddress = new(email);
            GetAthleteDto? athlete = await athleteService.GetAthleteByEmail(emailAddress);

            if (athlete == null)
                return NotFound(new { error = "Athlete not found" });

            return Ok(athlete);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve athlete", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    /// <summary>
    /// Update username
    /// </summary>
    [Authorize]
    [HttpPut("username")]
    public async Task<IActionResult> UpdateUsername([FromBody] UpdateUsernameDto dto)
    {
        try
        {
            await athleteService.UpdateUsernameAsync(dto);

            return Ok(new { message = "Username updated successfully" });
        }
        catch (AthleteNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to update username", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [Authorize]
    [HttpPut("changepassword")]
    public async Task<IActionResult> ChangePassword([FromBody] UpdatePasswordDto dto)
    {
        try
        {
            await athleteService.UpdateAthletePasswordAsync(dto);
            return Ok(new { message = "Password updated successfully" });
        }
        catch (AthleteNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to update password", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    /// <summary>
    /// Delete your own account. The target is taken from the authenticated identity,
    /// so a user can never delete anyone else's account.
    /// </summary>
    [Authorize]
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteOwnAccount()
    {
        try
        {
            string email = User.FindFirstValue(ClaimTypes.Email) ?? throw new InvalidOperationException("User email not found");
            EmailAddress emailAddress = new(email);
            await athleteService.DeleteAthleteByEmailAsync(emailAddress);
            return Ok(new { message = "Account deleted successfully" });
        }
        catch (AthleteNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to delete account", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    /// <summary>
    /// Delete any athlete by email (instructor/administrator only).
    /// </summary>
    [Authorize(Roles = "Instructor,Administrator")]
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