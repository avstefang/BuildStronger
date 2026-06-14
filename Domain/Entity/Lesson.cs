using System;

namespace Domain.Entity;

public class Lesson
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Workout Workout { get; private set; }
    public Schedule Schedule { get; private set; }
    public int CustomDuration { get; private set; }
    public int MaxCapacity { get; private set; }
    public Instructor? Instructor { get; private set; } = null;
    public Room Room { get; private set; } = null!;

    private Lesson() { }

    public Lesson(Workout workout, Schedule schedule, int maxCapacity, Room room, int customDuration = 0)
    {
        Workout = workout;
        Schedule = schedule;
        MaxCapacity = maxCapacity;
        CustomDuration = customDuration;

        if (maxCapacity > room.Capacity)
            throw new ArgumentException("Max capacity cannot exceed room capacity.");

        Room = room;
    }

    public void AssignInstructor(Instructor instructor)
    {
        Instructor = instructor;
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

    public void UpdateSchedule(Schedule schedule)
    {
        Schedule = schedule;
    }

    public TimeOnly GetEndTime()
    {
        int totalDuration = CustomDuration > 0 ? CustomDuration : Workout.Duration.Minutes;
        return Schedule.StartTime.Add(TimeSpan.FromMinutes(totalDuration));
    }
}