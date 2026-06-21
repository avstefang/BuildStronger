using System;

namespace Domain.Entity;

/// <summary>
/// Links a <see cref="Reservation"/> to the specific <see cref="EquipmentSpot"/> (e.g. a spinning
/// bike) the athlete chose. Maps to the equipmentSpot_reservation join table; only spinning-style
/// lessons (whose workout has equipment) get a row here.
/// </summary>
public class EquipmentSpotReservation
{
    public Guid ReservationId { get; private set; }
    public Guid LessonId { get; private set; }
    public Guid AthleteId { get; private set; }
    public EquipmentSpot EquipmentSpot { get; private set; } = null!;
    public DateTime ReservedAt { get; private set; } = DateTime.Now;

    private EquipmentSpotReservation() { }

    public EquipmentSpotReservation(Guid reservationId, Guid lessonId, Guid athleteId, EquipmentSpot equipmentSpot)
    {
        ReservationId = reservationId;
        LessonId = lessonId;
        AthleteId = athleteId;
        EquipmentSpot = equipmentSpot;
    }
}
