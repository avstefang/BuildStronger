using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface IEquipmentRepository : IRepository<Equipment, Guid>
{
    Task<Equipment> GetEquipmentByIdAsync(Guid id);
    Task<Equipment> GetEquipmentByNameAsync(string equipmentName);
    Task<IEnumerable<Equipment>> GetAllEquipmentsAsync();
    Task AddEquipmentAsync(Equipment equipment);
    Task DeleteEquipmentAsync(Guid equipmentId);
}
