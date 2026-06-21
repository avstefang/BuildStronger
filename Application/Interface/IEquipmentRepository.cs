using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface IEquipmentRepository
{
    Task<Equipment> GetEquipmentByIdAsync(Guid id);
    Task<Equipment> GetEquipmentByNameAsync(string equipmentName);
    Task<IEnumerable<Equipment>> GetAllEquipmentAsync();
    Task AddEquipmentAsync(Equipment equipment);
    Task DeleteEquipmentAsync(Guid equipmentId);
}
