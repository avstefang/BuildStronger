using Application.Dto;
using Domain.Entity;

namespace Application.Mapping;

public static class LessonMapping
{
    public static GetLessonDto ToDto(this Lesson lesson) => new()
    {
        Id = lesson.Id,
        Workout = lesson.Workout.ToDto(),
        Schedule = lesson.Schedule.ToDto(),
        Room = lesson.Room.ToDto(),
        Instructor = lesson.Instructor?.Athlete.ToSummaryDto(),
        CustomDuration = lesson.CustomDuration,
        MaxCapacity = lesson.MaxCapacity,
        EndTime = lesson.GetEndTime()
    };
}
