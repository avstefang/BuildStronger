using Application.Dto;
using Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservationController(ReservationService reservationService) : ControllerBase
{
    [HttpGet("lesson/{lessonId:guid}")]
    [Authorize(Roles = "Instructor,Administrator")]
    public async Task<IActionResult> GetLessonReservations([FromRoute] Guid lessonId)
    {
        try
        {
            IEnumerable<GetReservationDto>? reservations = await reservationService.GetLessonReservationsAsync(lessonId);
            return Ok(reservations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve reservations", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [Authorize]
    [HttpGet("athlete")]
    public async Task<IActionResult> GetAthleteReservations()
    {
        try
        {
            string email = User.FindFirstValue(ClaimTypes.Email) ?? throw new InvalidOperationException("User email not found");
            IEnumerable<GetReservationDto>? reservations = await reservationService.GetAthleteReservationsAsync(email);
            return Ok(reservations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve reservations", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ReserveLesson([FromBody] AddReservationDto dto)
    {
        try
        {
            GetReservationDto reservation = await reservationService.ReserveLessonAsync(dto);
            return Created(string.Empty, reservation);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to create reservation", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [Authorize]
    [HttpPut("{reservationId:guid}/cancel")]
    public async Task<IActionResult> CancelReservation([FromRoute] Guid reservationId)
    {
        try
        {
            string email = User.FindFirstValue(ClaimTypes.Email) ?? throw new InvalidOperationException("User email not found");
            GetReservationDto reservation = await reservationService.CancelReservationAsync(reservationId, email);
            return Ok(reservation);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to cancel reservation", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [HttpPost("process-waitlist")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> ProcessWaitlist()
    {
        try
        {
            await reservationService.AutoAcceptWaitlistReservationsAsync();
            return Ok(new { message = "Waitlist processed successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to process waitlist", details = ex.InnerException?.Message ?? ex.Message });
        }
    }
}
