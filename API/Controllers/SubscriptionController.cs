using Application.Dto;
using Application.Service;
using Domain.Exception;
using Domain.Value_object;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubscriptionController(SubscriptionService subscriptionService) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddSubscription([FromBody] AddSubscriptionDto dto)
    {
        try
        {
            string email = User.FindFirstValue(ClaimTypes.Email) ?? throw new InvalidOperationException("User email not found");
            GetAthleteDto athlete = await subscriptionService.RegisterSubscriptionAsync(dto, email);
            return Created(string.Empty, athlete);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to add subscription", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [Authorize]
    [HttpPut("change")]
    public async Task<IActionResult> ChangeSubscription([FromBody] AddSubscriptionDto dto)
    {
        try
        {
            string email = User.FindFirstValue(ClaimTypes.Email) ?? throw new InvalidOperationException("User email not found");
            GetAthleteDto athlete = await subscriptionService.ChangeSubscriptionAsync(dto, email);
            return Ok(athlete);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to change subscription", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [Authorize]
    [HttpPut("cancel")]
    public async Task<IActionResult> CancelSubscription()
    {
        try
        {
            string email = User.FindFirstValue(ClaimTypes.Email) ?? throw new InvalidOperationException("User email not found");
            await subscriptionService.CancelSubscriptionAsync(email);
            return Ok(new { message = "Subscription cancelled successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to cancel subscription", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [Authorize]
    [HttpGet("subscriptionstatus")]
    public async Task<IActionResult> GetSubscriptionStatus()
    {
        try
        {
            string email = User.FindFirstValue(ClaimTypes.Email) ?? throw new InvalidOperationException("User email not found");
            EmailAddress emailAddress = new(email);
            IEnumerable<GetSubscriptionDto>? subscriptions = await subscriptionService.GetAthleteSubscriptionsAsync(emailAddress);
            GetSubscriptionDto? subscription = subscriptions?.FirstOrDefault();
            if (subscription == null)
            {
                return NotFound(new { error = $"Athlete with email '{email}' has no active subscription" });
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

    /// <summary>
    /// Get athlete's subscriptions
    /// </summary>
    [Authorize]
    [HttpGet("subscriptions")]
    public async Task<IActionResult> GetSubscriptions()
    {
        try
        {
            string email = User.FindFirstValue(ClaimTypes.Email) ?? throw new InvalidOperationException("User email not found");
            EmailAddress emailAddress = new(email);
            IEnumerable<GetSubscriptionDto>? subscriptions = await subscriptionService.GetAthleteSubscriptionsAsync(emailAddress);
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
}
