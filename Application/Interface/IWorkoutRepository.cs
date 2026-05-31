using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface IWorkoutRepository : IRepository<Workout>
{
    Task<Workout> GetWorkoutAsync(Workout workout);
    Task<IEnumerable<Workout>> GetAllWorkoutsAsync();
    Task AddWorkoutAsync(Workout workout);
    Task UpdateWorkoutAsync(Workout workout);
    Task DeleteWorkoutAsync(Workout workout);
}
