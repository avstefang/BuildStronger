using System;
using Domain.Value_object;
using Domain.Exception;

namespace Domain.Entity;

public class EquipmentSpot
{
    private EquipmentSpot() { }

    public EquipmentSpot(Equipment equipment, EquipmentRoom equipmentRoom, EquipmentPosition equipmentPosition)
    {
        Equipment = equipment;
        EquipmentRoom = equipmentRoom;
        EquipmentPosition = equipmentPosition;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Equipment Equipment { get; private set; } = null!;
    public EquipmentRoom EquipmentRoom { get; private set; } = null!;
    public EquipmentPosition EquipmentPosition { get; private set; } = null!;

    public void UpdateRoom(EquipmentRoom newRoom)
    {
        EquipmentRoom = newRoom;
    }

    public void UpdatePosition(EquipmentPosition newPosition)
    {
        EquipmentPosition = newPosition;
    }
}