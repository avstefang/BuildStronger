using Application.Interface;
using Domain.Entity;
using Infrastructure.Context_model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class WorkoutRepository(LessonDbContext dbContext) : Repository<Workout, Guid>(dbContext), IWorkoutRepository
{
    public async Task<Workout?> GetWorkoutByNameAsync(string workoutName)
    {
        return await DbContext.Set<Workout>()
            .Include(w => w.Equipment)
            .FirstOrDefaultAsync(w => w.Name == workoutName);
    }

    public async Task<Workout?> GetWorkoutByIdAsync(Guid workoutId)
    {
        return await DbContext.Set<Workout>()
            .Include(w => w.Equipment)
            .FirstOrDefaultAsync(w => w.Id == workoutId);
    }

    public async Task<IEnumerable<Workout>?> GetAllWorkoutsAsync()
    {
        return await DbContext.Set<Workout>()
            .Include(w => w.Equipment)
            .ToListAsync();
    }

    public async Task AddWorkoutAsync(Workout workout) => await AddAsync(workout);

    public async Task UpdateWorkoutAsync(Workout workout) => await UpdateAsync(workout);

    public async Task DeleteWorkoutAsync(Guid workoutId) => await DeleteAsync(workoutId);
}
