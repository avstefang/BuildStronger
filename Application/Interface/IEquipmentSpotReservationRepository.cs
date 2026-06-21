using Domain.Entity;

namespace Application.Interface;

public interface IEquipmentSpotReservationRepository
{
    Task AddEquipmentSpotReservationAsync(EquipmentSpotReservation equipmentSpotReservation);
    Task<IEnumerable<EquipmentSpotReservation>?> GetByLessonIdAsync(Guid lessonId);
    Task DeleteByReservationIdAsync(Guid reservationId);
}
