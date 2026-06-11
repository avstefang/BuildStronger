using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface IRoomRepository : IRepository<Room, Guid>
{
    Task<Room> GetRoomByIdAsAsync(Guid roomId);
    Task<IEnumerable<Room>> GetAllRoomsAsync();
    Task AddRoomAsync(Room room);
    Task UpdateRoomAsync(Room room);
    Task DeleteRoomAsync(Guid roomId);
}
