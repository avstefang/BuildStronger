using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class CreateWorkoutDto
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int DurationInMinutes { get; private set; }
    public List<Guid> EquipmentIds { get; private set; } = [];

    public CreateWorkoutDto(string name, string description, int durationInMinutes)
    {
        Name = name;
        Description = description;
        DurationInMinutes = durationInMinutes;
    }

    public CreateWorkoutDto(string name, string description, int durationInMinutes, List<Guid> equipmentIds)
    {
        Name = name;
        Description = description;
        DurationInMinutes = durationInMinutes;
        EquipmentIds = equipmentIds;
    }
}