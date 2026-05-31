using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface IEquipmentRepository : IRepository<Equipment>
{
    Task<Equipment> GetEquipmentAsync(Equipment equipment);
    Task<IEnumerable<Equipment>> GetAllEquipmentsAsync();
    Task AddEquipmentAsync(Equipment equipment);
    Task UpdateEquipmentAsync(Equipment equipment);
    Task DeleteEquipmentAsync(Equipment equipment);
}
