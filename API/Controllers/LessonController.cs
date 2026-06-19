using Application.Dto;
using Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonController(LessonService lessonService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllLessons()
    {
        try
        {
            IEnumerable<GetLessonDto>? lessons = await lessonService.GetAllLessonsAsync();
            if (null == lessons)
                NotFound("No lessons are found");

            return Ok(lessons);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve lessons", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [HttpGet("workout/{workoutId:guid}")]
    public async Task<IActionResult> GetLessonsByWorkout([FromRoute] Guid workoutId)
    {
        try
        {
            IEnumerable<GetLessonDto>? lessons = await lessonService.GetLessonsByWorkoutIdAsync(workoutId);
            return Ok(lessons);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve lessons", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [HttpGet("workout/{workoutId:guid}/upcoming")]
    public async Task<IActionResult> GetUpcomingLessonsByWorkout([FromRoute] Guid workoutId)
    {
        try
        {
            IEnumerable<GetLessonDto>? lessons = await lessonService.GetCurrentOrFutureLessonsByWorkoutIdAsync(workoutId);
            return Ok(lessons);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve upcoming lessons", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [HttpGet("instructor/{instructorId:guid}")]
    public async Task<IActionResult> GetLessonsByInstructor([FromRoute] Guid instructorId)
    {
        try
        {
            IEnumerable<GetLessonDto>? lessons = await lessonService.GetLessonsByInstructorIdAsync(instructorId);
            return Ok(lessons);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve lessons for instructor", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Instructor,Administrator")]
    public async Task<IActionResult> CreateLesson([FromBody] CreateLessonDto dto)
    {
        try
        {
            GetLessonDto? lesson = await lessonService.CreateLessonAsync(dto);
            return Created(string.Empty, lesson);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to create lesson", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [HttpPut]
    [Authorize(Roles = "Instructor,Administrator")]
    public async Task<IActionResult> UpdateLesson([FromBody] UpdateLessonDto dto)
    {
        try
        {
            GetLessonDto lesson = await lessonService.UpdateLessonAsync(dto);
            return Ok(lesson);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to update lesson", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Instructor,Administrator")]
    public async Task<IActionResult> DeleteLesson([FromRoute] Guid id)
    {
        try
        {
            await lessonService.DeleteLessonAsync(id);
            return Ok(new { message = "Lesson deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to delete lesson", details = ex.InnerException?.Message ?? ex.Message });
        }
    }
}
