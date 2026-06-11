using Domain.Value_object;
using System;

namespace Domain.Entity;

public class EquipmentRoom(Room room, EquipmentLayout maxSpot)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Room Room { get; private set; } = room;
    public EquipmentLayout MaxSpot { get; set; } = maxSpot;

    public void UpdateEquipmentRoom(Room room, EquipmentLayout maxSpot)
    {
        if (room == Room && maxSpot == MaxSpot)
        {
            return;
        }

        Room = room;
        MaxSpot = maxSpot;
    }
}