using Application.Service;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomController(RoomService roomService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllRooms()
    {
        try
        {
            IEnumerable<Room>? rooms = await roomService.GetAllRoomsAsync();
            return Ok(rooms);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve rooms", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRoomById([FromRoute] Guid id)
    {
        try
        {
            Room? room = await roomService.GetRoomByIdAsync(id);
            if (room == null) return NotFound(new { error = $"Room {id} not found" });
            return Ok(room);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve room", details = ex.InnerException?.Message ?? ex.Message });
        }
    }
}
