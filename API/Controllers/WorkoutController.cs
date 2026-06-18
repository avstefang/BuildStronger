using Application.Dto;
using Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkoutController(WorkoutService workoutService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllWorkouts()
    {
        try
        {
            IEnumerable<GetWorkoutDto> workouts = await workoutService.GetAllWorkoutsAsync();
            return Ok(workouts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve workouts", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetWorkoutById([FromRoute] Guid id)
    {
        try
        {
            GetWorkoutDto? workout = await workoutService.GetWorkoutByIdAsync(id);
            if (workout == null) return NotFound(new { error = $"Workout {id} not found" });
            return Ok(workout);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve workout", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Instructor,Administrator")]
    public async Task<IActionResult> CreateWorkout([FromBody] CreateWorkoutDto dto)
    {
        try
        {
            await workoutService.CreateWorkoutAsync(dto);
            return Created(string.Empty, new { message = "Workout created successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to create workout", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Instructor,Administrator")]
    public async Task<IActionResult> UpdateWorkout([FromRoute] Guid id, [FromBody] UpdateWorkoutDto dto)
    {
        try
        {
            await workoutService.UpdateWorkoutAsync(id, dto);
            return Ok(new { message = "Workout updated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to update workout", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Instructor,Administrator")]
    public async Task<IActionResult> DeleteWorkout([FromRoute] Guid id)
    {
        try
        {
            await workoutService.DeleteWorkoutAsync(id);
            return Ok(new { message = "Workout deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to delete workout", details = ex.InnerException?.Message ?? ex.Message });
        }
    }
}
