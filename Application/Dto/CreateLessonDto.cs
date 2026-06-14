using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class CreateLessonDto(Guid workoutId, Schedule schedule, Guid roomId, int maxCapacity, int customDuration = 0)
{
    public Guid WorkoutId { get; private set; } = workoutId;
    public Schedule Schedule { get; private set; } = schedule;
    public Guid RoomId { get; private set; } = roomId;
    public int MaxCapacity { get; private set; } = maxCapacity;
    public int CustomDuration { get; private set; } = customDuration;

}