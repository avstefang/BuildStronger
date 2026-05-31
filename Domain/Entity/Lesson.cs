using System;

namespace Domain.Entity;

public class Lesson(Workout workout, int maxCapacity, Room room, List<Equipment>? equipment, int customDuration = 0)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Workout Workout { get; private set; } = workout;
    public int CustomDuration { get; private set; } = customDuration;
    public int MaxCapacity { get; private set; } = maxCapacity;
    public Instructor? Instructor { get; private set; }
    public Room Room { get; set; } = room;
    public List<Equipment>? Equipment { get; set; } = equipment;

    public void AssignInstructor(Instructor instructor)
    {
        Instructor = instructor ?? throw new ArgumentNullException(nameof(instructor), "Instructor cannot be null.");
    }

    public void UpdateCustomDuration(int duration)
    {
        if (duration < 0)
            throw new ArgumentOutOfRangeException(nameof(duration), "Custom duration cannot be negative.");

        CustomDuration = duration;
    }

    public void UpdateMaxCapacity(int capacity)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Max capacity cannot be negative.");

        MaxCapacity = capacity;
    }
}