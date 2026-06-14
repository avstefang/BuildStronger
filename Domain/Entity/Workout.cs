using System;
using Domain.Value_object;

namespace Domain.Entity;

public class Workout
{
    private Workout() { }

    public Workout(string name, string description, Duration duration)
    {
        Name = name;
        Description = description;
        Duration = duration;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Duration Duration { get; private set; } = null!;
    public List<Equipment> Equipment { get; set; } = [];

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

    public void AddEquipment(Equipment equipment)
    {
        Equipment.Add(equipment);
    }
}
