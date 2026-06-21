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
    [HttpGet("credits")]
    public async Task<IActionResult> GetCredits()
    {
        try
        {
            string email = User.FindFirstValue(ClaimTypes.Email) ?? throw new InvalidOperationException("User email not found");
            GetCreditStatusDto? credits = await reservationService.GetCreditStatusAsync(email);
            if (credits is null)
                return NotFound(new { error = "No active subscription" });

            return Ok(credits);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve credits", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    // Any member may see who else is attending a lesson (username + photo only).
    [Authorize]
    [HttpGet("lesson/{lessonId:guid}/participants")]
    public async Task<IActionResult> GetLessonParticipants([FromRoute] Guid lessonId)
    {
        try
        {
            string email = User.FindFirstValue(ClaimTypes.Email) ??
                throw new InvalidOperationException("User email not found");
            IEnumerable<GetLessonParticipantDto> participants = await reservationService.GetLessonParticipantsAsync(lessonId, email);
            return Ok(participants);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve participants", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [Authorize]
    [HttpGet("lesson/{lessonId:guid}/spots")]
    public async Task<IActionResult> GetLessonBookedSpots([FromRoute] Guid lessonId)
    {
        try
        {
            IEnumerable<GetBookedSpotDto> spots = await reservationService.GetBookedSpotsAsync(lessonId);
            return Ok(spots);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve booked spots", details = ex.InnerException?.Message ?? ex.Message });
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
            await reservationService.CancelReservationAsync(reservationId);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to cancel reservation", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    // Automatic geofence check-in from the app: marks the signed-in member present for their reservation.
    [Authorize]
    [HttpPut("{reservationId:guid}/checkin")]
    public async Task<IActionResult> CheckIn([FromRoute] Guid reservationId)
    {
        try
        {
            string email = User.FindFirstValue(ClaimTypes.Email) ?? throw new InvalidOperationException("User email not found");
            await reservationService.CheckInAsync(reservationId, email);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to check in", details = ex.InnerException?.Message ?? ex.Message });
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
