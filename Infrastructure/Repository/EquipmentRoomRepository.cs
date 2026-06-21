using Application.Interface;
using Domain.Entity;
using Infrastructure.Context_model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class EquipmentRoomRepository(RoomDbContext dbContext) : Repository<EquipmentRoom, Guid>(dbContext), IEquipmentRoomRepository
{
    public async Task<EquipmentRoom> GetEquipmentRoomByIdAsync(Guid id)
    {
        return await DbContext.Set<EquipmentRoom>()
            .Include(e => e.Room)
            .FirstOrDefaultAsync(e => e.Id == id)
            ?? throw new InvalidOperationException($"EquipmentRoom with ID {id} not found.");
    }

    public async Task<EquipmentRoom> GetEquipmentRoomByNameAsync(string roomName)
    {
        return await DbContext.Set<EquipmentRoom>()
            .Include(e => e.Room)
            .FirstOrDefaultAsync(e => e.Room.Name == roomName)
            ?? throw new InvalidOperationException($"EquipmentRoom for room '{roomName}' not found.");
    }

    public async Task<IEnumerable<EquipmentRoom>> GetAllEquipmentRoomsAsync()
    {
        return await DbContext.Set<EquipmentRoom>()
            .Include(e => e.Room)
            .ToListAsync();
    }

    public async Task AddEquipmentRoomAsync(EquipmentRoom equipmentRoom) => await AddAsync(equipmentRoom);

    public async Task UpdateEquipmentRoomAsync(EquipmentRoom equipmentRoom) => await UpdateAsync(equipmentRoom);

    public async Task DeleteEquipmentRoomAsync(Guid equipmentRoomId) => await DeleteAsync(equipmentRoomId);
}
