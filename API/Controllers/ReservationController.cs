using Application.Dto;
using Application.Service;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            IEnumerable<Reservation>? reservations = await reservationService.GetLessonReservationsAsync(lessonId);
            return Ok(reservations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve reservations", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [HttpGet("athlete/{email}")]
    public async Task<IActionResult> GetAthleteReservations([FromRoute] string email)
    {
        try
        {
            IEnumerable<Reservation>? reservations = await reservationService.GetAthleteReservationsAsync(email);
            return Ok(reservations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve reservations", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ReserveLesson([FromBody] AddReservationDto dto)
    {
        try
        {
            Reservation reservation = await reservationService.ReserveLessonAsync(dto);
            return Created(string.Empty, reservation);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to create reservation", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [HttpPut("{reservationId:guid}/cancel")]
    public async Task<IActionResult> CancelReservation([FromRoute] Guid reservationId)
    {
        try
        {
            Reservation reservation = await reservationService.CancelReservationAsync(reservationId);
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
