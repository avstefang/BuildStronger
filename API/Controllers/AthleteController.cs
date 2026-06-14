using Application.Dto;
using Application.Interface;
using Application.Service;
using Domain.Entity;
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
    /// Get athlete by ID (admin or authenticated user only)
    /// </summary>
    [Authorize]
    [HttpGet("{email}")]
    public async Task<IActionResult> GetAthleteByEmail([FromRoute] string email)
    {
        try
        {
            EmailAddress emailAddress = new(email);
            Athlete? athlete = await athleteService.GetAthleteByEmail(emailAddress);

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

    /// <summary>
    /// Get athlete's subscriptions
    /// </summary>
    [Authorize]
    [HttpGet("{email}/subscriptions")]
    public async Task<IActionResult> GetSubscriptions([FromRoute] string email)
    {
        try
        {
            EmailAddress emailAddress = new(email);
            IEnumerable<Subscription>? subscriptions = await athleteService.GetAthleteSubscriptionsAsync(emailAddress);
            if (subscriptions == null)
            {
                return NotFound(new { error = "Athlete not found or no subscriptions" });
            }

            return Ok(subscriptions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve subscriptions", details = ex.InnerException?.Message ?? ex.Message });
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

    [Authorize]
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

    [Authorize]
    [HttpGet("{email}/GetSubscriptionStatus")]
    public async Task<IActionResult> GetSubscriptionStatus([FromRoute] string email)
    {
        try
        {
            EmailAddress emailAddress = new(email);
            Subscription? subscription = await athleteService.GetActiveSubscriptionAsync(emailAddress);
            if (subscription == null)
            {
                return NotFound(new { error = "No active subscription found" });
            }   
            return Ok(subscription.Status);
        }
        catch (AthleteNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve subscription status", details = ex.InnerException?.Message ?? ex.Message });
        }
    }
}