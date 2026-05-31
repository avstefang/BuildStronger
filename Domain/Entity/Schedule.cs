using Domain.Value_object;
using System;

namespace Domain.Entity;

public class Schedule(TimeSpan startTime, Lesson lesson, DayOfWeek startDay, Repetition repetition)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public TimeSpan StartTime { get; private set; } = startTime;
    public Lesson Lesson { get; private set; } = lesson;
    public DayOfWeek StartDay { get; private set; } = startDay;
    public Repetition Repetition { get; private set; } = repetition;
    public DateTime RegisteredAt { get; private set; } = DateTime.Now;

    public void ChangeStartTime(TimeSpan newStartTime)
    {
        StartTime = newStartTime;
    }

    public void ChangeLesson(Lesson newLesson)
    {
        Lesson = newLesson;
    }

    public void ChangeStartDay(DayOfWeek newStartDay)
    {
        StartDay = newStartDay;
    }

    public void ChangeRepetition(Repetition newRepetition)
    {
        Repetition = newRepetition;
    }
}