using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Value_object;

public sealed record class LessonTime
{
    public DateTime Start { get; }
    public Duration Duration { get; }

    public DateTime End => Start.AddMinutes(Duration.Minutes);

    public LessonTime(DateTime start, Duration duration)
    {
        Start = start;
        Duration = duration;
    }
}