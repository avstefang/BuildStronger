using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface IEquipmentSpotRepository : IRepository<EquipmentSpot>
{
    Task<EquipmentSpot> GetEquipmentSpotAsync(EquipmentSpot entity);
    Task<IEnumerable<EquipmentSpot>> GetAllEquipmentSpotsAsync();
    Task AddEquipmentSpotAsync(EquipmentSpot entity);
    Task UpdateEquipmentSpotAsync(EquipmentSpot entity);
    Task DeleteEquipmentSpotAsync(EquipmentSpot entity);
}
