using System;

namespace Domain.Entity;

public class Lesson(Workout workout, Schedule schedule, int maxCapacity, int customDuration = 0)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Workout Workout { get; private set; } = workout;
    public Schedule Schedule { get; private set; } = schedule;
    public int CustomDuration { get; private set; } = customDuration;
    public int MaxCapacity { get; private set; } = maxCapacity;
    public Instructor? Instructor { get; private set; }

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