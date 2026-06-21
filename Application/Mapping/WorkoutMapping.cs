using Application.Dto;
using Domain.Entity;

namespace Application.Mapping;

public static class WorkoutMapping
{
    public static GetWorkoutDto ToDto(this Workout workout) => new()
    {
        Id = workout.Id,
        Name = workout.Name,
        Description = workout.Description,
        DurationInMinutes = workout.Duration.Minutes,
        Equipment = workout.Equipment.Select(equipment => equipment.ToDto()).ToList()
    };
}
