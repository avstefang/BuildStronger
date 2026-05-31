using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface IEquipmentRoomRepository : IRepository<EquipmentRoom>
{
    Task<EquipmentRoom> GetEquipmentRoomAsync(EquipmentRoom entity);
    Task<IEnumerable<EquipmentRoom>> GetAllEquipmentRoomsAsync();
    Task AddEquipmentRoomAsync(EquipmentRoom entity);
    Task UpdateEquipmentRoomAsync(EquipmentRoom entity);
    Task DeleteEquipmentRoomAsync(EquipmentRoom entity);
}
