using System;
using Domain.Value_object;

namespace Domain.Entity;

public class Workout(string name, string description, Duration duration, List<Equipment>? equipment = null)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public Duration Duration { get; private set; } = duration;
    public List<Equipment>? Equipment { get; set; } = equipment;

    public void UpdateName(string name)
    {
        Name = name;
    }

    public void UpdateDescription(string description)
    {
        Description = description;
    }

    public void UpdateDuration(Duration duration)
    {
        Duration = duration;
    }
}
