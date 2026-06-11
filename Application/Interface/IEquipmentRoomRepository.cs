using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface IEquipmentRoomRepository : IRepository<EquipmentRoom, Guid>
{
    Task<EquipmentRoom> GetEquipmentRoomByIdAsync(Guid id);
    Task<EquipmentRoom> GetEquipmentRoomByNameAsync(string roomName);
    Task<IEnumerable<EquipmentRoom>> GetAllEquipmentRoomsAsync();
    Task AddEquipmentRoomAsync(EquipmentRoom equipmentRoom);
    Task UpdateEquipmentRoomAsync(EquipmentRoom equipmentRoom);
    Task DeleteEquipmentRoomAsync(Guid equipmentRoomId);
}
