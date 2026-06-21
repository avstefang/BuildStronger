using Application.Dto;
using Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InstructorController(InstructorService instructorService) : ControllerBase
{
    /// <summary>List all instructors (any signed-in user, e.g. to show trainer info).</summary>
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllInstructors()
    {
        try
        {
            IEnumerable<GetInstructorDto> instructors = await instructorService.GetAllInstructorsAsync();
            return Ok(instructors);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve instructors", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetInstructorById([FromRoute] Guid id)
    {
        try
        {
            GetInstructorDto instructor = await instructorService.GetInstructorByIdAsync(id);
            return Ok(instructor);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve instructor", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    /// <summary>Create a new instructor from name + email (staff only).</summary>
    [Authorize(Roles = "Instructor,Administrator")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInstructorDto dto)
    {
        try
        {
            await instructorService.CreateInstructorAsync(dto);
            return Created(string.Empty, new { message = "Instructor created" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to create instructor", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    /// <summary>Update an instructor's name, email and username (staff only).</summary>
    [Authorize(Roles = "Instructor,Administrator")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateInstructorDto dto)
    {
        try
        {
            await instructorService.UpdateInstructorAsync(id, dto);
            return Ok(new { message = "Instructor updated" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to update instructor", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    /// <summary>
    /// Removes someone as an instructor by demoting them back to a regular member. This keeps the
    /// athlete account intact — it only changes the role, it does not delete the athlete.
    /// </summary>
    [Authorize(Roles = "Instructor,Administrator")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Demote([FromRoute] Guid id)
    {
        try
        {
            await instructorService.DemoteInstructorAsync(id);
            return Ok(new { message = "Instructor demoted to member" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to demote instructor", details = ex.InnerException?.Message ?? ex.Message });
        }
    }
}
