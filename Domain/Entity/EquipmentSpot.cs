using System;
using Domain.Value_object;
using Domain.Exception;

namespace Domain.Entity;

public class EquipmentSpot
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Lesson Lesson { get; private set; }
    public Equipment Equipment { get; private set; }
    public EquipmentRoom EquipmentRoom { get; private set; }
    public EquipmentPosition EquipmentPosition { get; private set; }
    public Reservation Reservation { get; private set; }

    public EquipmentSpot(Lesson lesson, Equipment equipment, EquipmentRoom equipmentRoom, EquipmentPosition equipmentPosition, Reservation reservation)
    {
        if (lesson?.Equipment?.Contains(equipment) == false)
            throw new DomainException("The equipment is not available for this lesson.");

        Lesson = lesson ?? throw new DomainException("The lesson cannot be null.");
        Equipment = equipment;
        EquipmentRoom = equipmentRoom;
        EquipmentPosition = equipmentPosition;
        Reservation = reservation;
    }
}