using Domain.Value_object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class UpdateScheduleDto(Guid id, TimeOnly startTime, DayOfWeek startDay, Repetition? repetition = null)
{
    public Guid Id { get; private set; } = id;
    public TimeOnly StartTime { get; private set; } = startTime;
    public DayOfWeek StartDay { get; private set; } = startDay;
    public Repetition? Repetition { get; private set; } = repetition;
}