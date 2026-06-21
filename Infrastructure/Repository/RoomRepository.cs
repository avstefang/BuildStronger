using Application.Interface;
using Domain.Entity;
using Infrastructure.Context_model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class RoomRepository(RoomDbContext dbContext) : Repository<Room, Guid>(dbContext), IRoomRepository
{
    public async Task<Room?> GetRoomByIdAsync(Guid roomId)
    {
        return await DbContext.Set<Room>().FindAsync(roomId);
    }

    public async Task<IEnumerable<Room>?> GetAllRoomsAsync()
    {
        return await DbContext.Set<Room>().ToListAsync();
    }

    public async Task AddRoomAsync(Room room) => await AddAsync(room);

    public async Task UpdateRoomAsync(Room room) => await UpdateAsync(room);

    public async Task DeleteRoomAsync(Guid roomId) => await DeleteAsync(roomId);
}
