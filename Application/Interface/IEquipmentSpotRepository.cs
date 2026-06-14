using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface IEquipmentSpotRepository
{
    Task<EquipmentSpot?> GetEquipmentSpotByIdAsync(Guid equipmentSpotId);
    Task<EquipmentSpot?> GetEquipmentSpotByNameAsync(string equipmentName);
    Task<IEnumerable<EquipmentSpot>?> GetAllEquipmentSpotsAsync();
    Task AddEquipmentSpotAsync(EquipmentSpot equipmentSpot);
    Task UpdateEquipmentSpotAsync(EquipmentSpot equipmentSpot);
    Task DeleteEquipmentSpotAsync(Guid equipmentSpotId);
}
