using Microsoft.AspNetCore.Mvc;
using Application.Service;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionPlanController(SubscriptionPlanService planService) : Controller
{
    [HttpGet("plans")]
    public async Task<IActionResult> GetAllSubscriptionPlans()
    {
        try
        {
            var plans = await planService.GetAllSubscriptionPlansAsync();
            return Ok(plans);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve subscription plans", details = ex.InnerException?.Message ?? ex.Message });
        }
    }
}