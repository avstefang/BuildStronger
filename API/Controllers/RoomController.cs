using Application.Dto;
using Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomController(RoomService roomService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllRooms()
    {
        try
        {
            IEnumerable<GetRoomDto> rooms = await roomService.GetAllRoomsAsync();
            return Ok(rooms);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve rooms", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRoomById([FromRoute] Guid id)
    {
        try
        {
            GetRoomDto? room = await roomService.GetRoomByIdAsync(id);
            if (room == null) return NotFound(new { error = $"Room {id} not found" });
            return Ok(room);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve room", details = ex.InnerException?.Message ?? ex.Message });
        }
    }
}
