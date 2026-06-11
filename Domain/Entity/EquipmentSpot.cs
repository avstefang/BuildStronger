using System;
using Domain.Value_object;
using Domain.Exception;

namespace Domain.Entity;

public class EquipmentSpot
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Lesson Lesson { get; private set; }
    public EquipmentRoom EquipmentRoom { get; private set; }
    public EquipmentPosition EquipmentPosition { get; private set; }
    public Reservation Reservation { get; private set; }

    public EquipmentSpot(Lesson lesson, EquipmentRoom equipmentRoom, EquipmentPosition equipmentPosition, Reservation reservation)
    {
        Lesson = lesson;
        EquipmentRoom = equipmentRoom;
        EquipmentPosition = equipmentPosition;
        Reservation = reservation;
    }

    public void UpdateRoom(EquipmentRoom newRoom)
    {
        EquipmentRoom = newRoom;
    }

    public void UpdatePosition(EquipmentPosition newPosition)
    {
        EquipmentPosition = newPosition;
    }

    public void UpdateReservation(Reservation newReservation)
    {
        Reservation = newReservation;
    }
}