using Application.Dto;
using Domain.Entity;

namespace Application.Mapping;

public static class ScheduleMapping
{
    public static GetScheduleDto ToDto(this Schedule schedule) => new()
    {
        Id = schedule.Id,
        StartTime = schedule.StartTime,
        StartDay = schedule.StartDay.ToString(),
        RepetitionCount = schedule.Repetition.RepetitionCount,
        RepetitionEndDate = schedule.Repetition.RepetitionEndDate,
        RegisteredAt = schedule.RegisteredAt
    };
}
