using Domain.Value_object;
using System;

namespace Domain.Entity;

public class EquipmentRoom(Room room, EquipmentLayout maxSpot)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Room Room { get; private set; } = room;
    public EquipmentLayout MaxSpot { get; set; } = maxSpot;
}