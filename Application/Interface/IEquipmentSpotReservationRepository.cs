using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface IEquipmentSpotReservationRepository : IRepository<EquipmentSpotReservation>
{
    Task<EquipmentSpotReservation> GetEquipmentSpotReservationAsync(EquipmentSpotReservation entity);
    Task<IEnumerable<EquipmentSpotReservation>> GetAllEquipmentSpotReservationsAsync();
    Task AddEquipmentSpotReservationAsync(EquipmentSpotReservation entity);
    Task UpdateEquipmentSpotReservationAsync(EquipmentSpotReservation entity);
    Task DeleteEquipmentSpotReservationAsync(EquipmentSpotReservation entity);
}
