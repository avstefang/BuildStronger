using Application.Interface;
using Application.Service;
using Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubscriptionsController(
    SubscriptionService subscriptionService,
    IAthleteRepository athleteRepository,
    ISubscriptionRepository subscriptionRepository
) : ControllerBase
{
    private readonly SubscriptionService _subscriptionService = subscriptionService;
    private readonly IAthleteRepository _athleteRepository = athleteRepository;
    private readonly ISubscriptionRepository _subscriptionRepository = subscriptionRepository;

    /// <summary>
    /// Register a new subscription for authenticated athlete
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> RegisterSubscription([FromBody] RegisterSubscriptionDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var athleteId))
                return Unauthorized(new { error = "Invalid token" });

            var athlete = await _athleteRepository.GetByIdAsync(athleteId);

            if (athlete == null)
                return NotFound(new { error = "Athlete not found" });

            var startDate = dto.StartDate.HasValue ? new DateOnly?(DateOnly.FromDateTime(dto.StartDate.Value)) : null;

            var updatedAthlete = await _subscriptionService.RegisterSubscriptionAsync(
                athlete,
                dto.SubscriptionName,
                startDate,
                dto.AutoRenew
            );

            var activeSubscription = updatedAthlete.GetActiveSubscription();

            return Ok(new
            {
                message = "Subscription registered successfully",
                subscription = new
                {
                    id = activeSubscription?.Id,
                    planName = activeSubscription?.SubscriptionPlan.Name,
                    status = activeSubscription?.Status.ToString(),
                    startDate = activeSubscription?.StartDate.ToString("yyyy-MM-dd"),
                    autoRenew = activeSubscription?.AutoRenew
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to register subscription", details = ex.Message });
        }
    }

    /// <summary>
    /// Change subscription to a different plan
    /// </summary>
    [HttpPost("change")]
    public async Task<IActionResult> ChangeSubscription([FromBody] ChangeSubscriptionDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var athleteId))
                return Unauthorized(new { error = "Invalid token" });

            var athlete = await _athleteRepository.GetByIdAsync(athleteId);

            if (athlete == null)
                return NotFound(new { error = "Athlete not found" });

            var startDate = dto.StartDate.HasValue ? new DateOnly?(DateOnly.FromDateTime(dto.StartDate.Value)) : null;

            var updatedAthlete = await _subscriptionService.ChangeSubscriptionAsync(
                athlete,
                dto.NewSubscriptionName,
                startDate,
                dto.AutoRenew
            );

            var activeSubscription = updatedAthlete.GetActiveSubscription();

            return Ok(new
            {
                message = "Subscription changed successfully",
                subscription = new
                {
                    id = activeSubscription?.Id,
                    planName = activeSubscription?.SubscriptionPlan.Name,
                    status = activeSubscription?.Status.ToString(),
                    startDate = activeSubscription?.StartDate.ToString("yyyy-MM-dd"),
                    autoRenew = activeSubscription?.AutoRenew
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to change subscription", details = ex.Message });
        }
    }

    /// <summary>
    /// Cancel current active subscription
    /// </summary>
    [HttpPost("cancel")]
    public async Task<IActionResult> CancelSubscription()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var athleteId))
                return Unauthorized(new { error = "Invalid token" });

            var athlete = await _athleteRepository.GetByIdAsync(athleteId);

            if (athlete == null)
                return NotFound(new { error = "Athlete not found" });

            var activeSubscription = athlete.GetActiveSubscription();
            if (activeSubscription == null)
                return BadRequest(new { error = "No active subscription to cancel" });

            await _subscriptionService.CancelSubscriptionAsync(athlete);

            return Ok(new { message = "Subscription cancelled successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to cancel subscription", details = ex.Message });
        }
    }

    /// <summary>
    /// Activate a subscription that is scheduled for future activation
    /// </summary>
    [HttpPost("{subscriptionId}/activate")]
    public async Task<IActionResult> ActivateSubscription(Guid subscriptionId)
    {
        try
        {
            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(subscriptionId);

            var result = await _subscriptionService.ActivateLatentSubscriptionAsync(subscription);

            if (!result)
                return BadRequest(new { error = "Subscription start date has already passed" });

            return Ok(new { message = "Subscription activated successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to activate subscription", details = ex.Message });
        }
    }

    /// <summary>
    /// Get all subscriptions (admin only - add [Authorize(Roles = "Admin")] when implementing role-based access)
    /// </summary>
    [HttpGet("all")]
    public async Task<IActionResult> GetAllSubscriptions()
    {
        try
        {
            var subscriptions = await _subscriptionRepository.GetAllSubscriptionsAsync();

            var result = subscriptions.Select(s => new
            {
                id = s.Id,
                planName = s.SubscriptionPlan.Name,
                status = s.Status.ToString(),
                startDate = s.StartDate.ToString("yyyy-MM-dd"),
                autoRenew = s.AutoRenew
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve subscriptions", details = ex.Message });
        }
    }
}

public class RegisterSubscriptionDto
{
    public string SubscriptionName { get; set; }
    public DateTime? StartDate { get; set; }
    public bool AutoRenew { get; set; } = false;
}

public class ChangeSubscriptionDto
{
    public string NewSubscriptionName { get; set; }
    public DateTime? StartDate { get; set; }
    public bool AutoRenew { get; set; } = false;
}
