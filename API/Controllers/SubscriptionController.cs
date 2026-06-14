using Application.Dto;
using Application.Service;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubscriptionController(SubscriptionService subscriptionService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddSubscription([FromBody] AddSubscriptionDto dto)
    {
        try
        {
            Athlete athlete = await subscriptionService.RegisterSubscriptionAsync(dto);
            return Created(string.Empty, athlete);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to add subscription", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [HttpPut("change")]
    public async Task<IActionResult> ChangeSubscription([FromBody] AddSubscriptionDto dto)
    {
        try
        {
            Athlete athlete = await subscriptionService.ChangeSubscriptionAsync(dto);
            return Ok(athlete);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to change subscription", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [HttpPut("{email}/cancel")]
    public async Task<IActionResult> CancelSubscription([FromRoute] string email)
    {
        try
        {
            await subscriptionService.CancelSubscriptionAsync(email);
            return Ok(new { message = "Subscription cancelled successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to cancel subscription", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [HttpPost("activate-latent")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> ActivateLatentSubscriptions()
    {
        try
        {
            await subscriptionService.ActivateLatentSubscriptionAsync();
            return Ok(new { message = "Latent subscriptions activated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to activate subscriptions", details = ex.InnerException?.Message ?? ex.Message });
        }
    }
}
