using Domain.Value_object;
using System;

namespace Domain.Entity;

public class Schedule(TimeOnly startTime, DayOfWeek startDay, Repetition? repetition = null)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public TimeOnly StartTime { get; private set; } = startTime;
    public DayOfWeek StartDay { get; private set; } = startDay;
    public Repetition Repetition { get; private set; } = repetition ?? new(1, null);
    public DateTime RegisteredAt { get; private set; } = DateTime.Now;

    public void ChangeStartTime(TimeOnly newStartTime)
    {
        StartTime = newStartTime;
    }

    public void ChangeStartDay(DayOfWeek newStartDay)
    {
        StartDay = newStartDay;
    }

    public void ChangeRepetition(Repetition newRepetition)
    {
        Repetition = newRepetition;
    }

    public DateTime StartDateTime()
    {
        // Calculate days until the Target StartDay starting from the day it was Registered
        int daysUntilStart = ((int)StartDay - (int)RegisteredAt.DayOfWeek + 7) % 7;

        // Use .Date to strip out the time-of-day before adding the specific StartTime
        return RegisteredAt.Date
            .AddDays(daysUntilStart)
            .Add(StartTime.ToTimeSpan());
    }

    public DateTime? GetNextOccurrence(DateTime? currentTime = null)
    {
        DateTime current = currentTime ?? DateTime.Now;
        DateTime startDateTime = StartDateTime();

        DateTime endDateTime;
        if (Repetition.RepetitionEndDate.HasValue)
        {
            endDateTime = Repetition.RepetitionEndDate.Value;
        }
        else
        {
            // Add 7 days for every repetition beyond the first one
            int extraWeeks = (Repetition.RepetitionCount ?? 1) - 1;
            endDateTime = startDateTime.AddDays(extraWeeks * 7);
        }

        if (current <= startDateTime) return startDateTime;
        if (current > endDateTime) return null;

        // Calculate how many EXACT 7-day cycles we need to jump forward from the start date
        double weeksSinceStart = (current - startDateTime).TotalDays / 7.0;
        int upcomingWeekCycle = (int)Math.Ceiling(weeksSinceStart);

        DateTime nextOccurrence = startDateTime.AddDays(upcomingWeekCycle * 7);
        return nextOccurrence <= endDateTime ? nextOccurrence : null;
    }

    public bool IsExpired()
    {
        DateTime? nextOccurrence = GetNextOccurrence(DateTime.Now);
        return !nextOccurrence.HasValue || DateTime.Now > nextOccurrence.Value;
    }
}