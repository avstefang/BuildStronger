using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface IWorkoutRepository
{
    Task<Workout?> GetWorkoutByNameAsync(string workoutName);
    Task<Workout?> GetWorkoutByIdAsync(Guid workoutId);
    Task<IEnumerable<Workout>?> GetAllWorkoutsAsync();
    Task AddWorkoutAsync(Workout workout);
    Task UpdateWorkoutAsync(Workout workout);
    Task DeleteWorkoutAsync(Guid workoutId);
}
