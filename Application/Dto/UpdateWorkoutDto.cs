using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class UpdateWorkoutDto(string name, string description, int durationInMinutes)
{
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public int DurationInMinutes { get; private set; } = durationInMinutes;
}