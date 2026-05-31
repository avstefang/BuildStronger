using Domain.Entity;
using Domain.Value_object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class ReadEquipmentSpotDto
{
    public Guid Id { get; set; }
    public required Athlete Athlete { get; set; }
    public required Lesson Lesson { get; set; }
    public required Equipment Equipment { get; set; }
    public required EquipmentRoom EquipmentRoom { get; set; }
    public required EquipmentPosition EquipmentPosition { get; set; }
    public required Reservation Reservation { get; set; }

    public static ReadEquipmentSpotDto Convert(EquipmentSpot equipmentSpot) => new ReadEquipmentSpotDto
    {
        Id = equipmentSpot.Id,
        Athlete = equipmentSpot.Athlete,
        Lesson = equipmentSpot.Lesson,
        Equipment = equipmentSpot.Equipment,
        EquipmentRoom = equipmentSpot.EquipmentRoom,
        EquipmentPosition = equipmentSpot.EquipmentPosition,
        Reservation = equipmentSpot.Reservation
    };
}