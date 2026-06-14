using Domain.Value_object;
using System;

namespace Domain.Entity;

public class EquipmentRoom
{
    private EquipmentRoom() { }

    public EquipmentRoom(Room room, EquipmentLayout maxSpot)
    {
        Room = room;
        MaxSpot = maxSpot;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Room Room { get; private set; } = null!;
    public EquipmentLayout MaxSpot { get; set; } = null!;

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