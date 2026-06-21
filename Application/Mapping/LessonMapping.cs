using Application.Dto;
using Domain.Entity;

namespace Application.Mapping;

public static class LessonMapping
{
    public static GetLessonDto ToDto(this Lesson lesson, int bookedCount = 0) => new()
    {
        Id = lesson.Id,
        Workout = lesson.Workout.ToDto(),
        Schedule = lesson.Schedule.ToDto(),
        Room = lesson.Room.ToDto(),
        Instructor = lesson.Instructor?.Athlete.ToSummaryDto(),
        CustomDuration = lesson.CustomDuration ?? 0,
        MaxCapacity = lesson.MaxCapacity,
        EndTime = lesson.GetEndTime(),
        BookedCount = bookedCount
    };
}
