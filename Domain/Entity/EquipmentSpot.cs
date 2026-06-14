using System;
using Domain.Value_object;
using Domain.Exception;

namespace Domain.Entity;

public class EquipmentSpot(Equipment equipment, EquipmentRoom equipmentRoom, EquipmentPosition equipmentPosition)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Equipment Equipment { get; private set; } = equipment;
    public EquipmentRoom EquipmentRoom { get; private set; } = equipmentRoom;
    public EquipmentPosition EquipmentPosition { get; private set; } = equipmentPosition;

    public void UpdateRoom(EquipmentRoom newRoom)
    {
        EquipmentRoom = newRoom;
    }

    public void UpdatePosition(EquipmentPosition newPosition)
    {
        EquipmentPosition = newPosition;
    }
}