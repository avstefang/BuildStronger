using Application.Dto;
using Domain.Entity;

namespace Application.Mapping;

public static class RoomMapping
{
    public static GetRoomDto ToDto(this Room room) => new()
    {
        Id = room.Id,
        Name = room.Name,
        Capacity = room.Capacity,
        LocationId = room.LocationId
    };
}
