using Application.Interface;
using Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AthletesController(IAthleteRepository athleteRepository) : ControllerBase
{
    private readonly IAthleteRepository _athleteRepository = athleteRepository;

    /// <summary>
    /// Get current authenticated athlete's profile
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var athleteId))
                return Unauthorized(new { error = "Invalid token" });

            var athlete = await _athleteRepository.GetByIdAsync(athleteId);

            if (athlete == null)
                return NotFound(new { error = "Athlete not found" });

            return Ok(new
            {
                id = athlete.Id,
                email = athlete.EmailAddress.Address,
                fullName = athlete.FullName.ToString(),
                role = athlete.Role.ToString(),
                username = athlete.Username,
                hasActiveSubscription = athlete.HasActiveSubscription()
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve profile", details = ex.Message });
        }
    }

    /// <summary>
    /// Get athlete by ID (admin or authenticated user only)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAthleteById(Guid id)
    {
        try
        {
            var athlete = await _athleteRepository.GetByIdAsync(id);

            if (athlete == null)
                return NotFound(new { error = "Athlete not found" });

            return Ok(new
            {
                id = athlete.Id,
                email = athlete.EmailAddress.Address,
                fullName = athlete.FullName.ToString(),
                role = athlete.Role.ToString(),
                username = athlete.Username
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve athlete", details = ex.Message });
        }
    }

    /// <summary>
    /// Update username
    /// </summary>
    [HttpPut("username")]
    public async Task<IActionResult> UpdateUsername([FromBody] UpdateUsernameDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var athleteId))
                return Unauthorized(new { error = "Invalid token" });

            var athlete = await _athleteRepository.GetByIdAsync(athleteId);

            if (athlete == null)
                return NotFound(new { error = "Athlete not found" });

            athlete.SetUsername(dto.Username);
            await _athleteRepository.UpdateAsync(athlete);

            return Ok(new { message = "Username updated successfully" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to update username", details = ex.Message });
        }
    }

    /// <summary>
    /// Get athlete's subscriptions
    /// </summary>
    [HttpGet("subscriptions")]
    public async Task<IActionResult> GetSubscriptions()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var athleteId))
                return Unauthorized(new { error = "Invalid token" });

            var athlete = await _athleteRepository.GetByIdAsync(athleteId);

            if (athlete == null)
                return NotFound(new { error = "Athlete not found" });

            var subscriptions = athlete.Subscriptions.Select(s => new
            {
                id = s.Id,
                planName = s.SubscriptionPlan.Name,
                status = s.Status.ToString(),
                startDate = s.StartDate.ToString("yyyy-MM-dd"),
                endDate = s.StartDate.AddMonths(s.SubscriptionPlan.DurationInMonths).ToString("yyyy-MM-dd"),
                autoRenew = s.AutoRenew
            });

            return Ok(new
            {
                athleteId = athlete.Id,
                subscriptions = subscriptions,
                hasActiveSubscription = athlete.HasActiveSubscription()
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve subscriptions", details = ex.Message });
        }
    }
}

public class UpdateUsernameDto
{
    public string Username { get; set; }
}
