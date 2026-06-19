using Application.Dto;
using Application.Interface;
using Domain.Entity;
using Infrastructure.Context_model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class LessonRepository(LessonDbContext dbContext) : Repository<Lesson, Guid>(dbContext), ILessonRepository
{
    private IQueryable<Lesson> LessonsWithIncludes() =>
        DbContext.Set<Lesson>()
            .Include(l => l.Workout).ThenInclude(w => w.Equipment)
            .Include(l => l.Schedule)
            .Include(l => l.Room)
            // A lesson must have a schedule to be usable; skip orphaned rows so a single
            // bad record doesn't break the whole endpoint (the mapping reads l.Schedule).
            .Where(l => l.Schedule != null);

    public async Task<Lesson?> GetLessonByIdAsync(Guid lessonId)
    {
        return await LessonsWithIncludes().FirstOrDefaultAsync(l => l.Id == lessonId);
    }

    public async Task<IEnumerable<Lesson>?> GetLessonsByWorkoutIdAsync(Guid workoutId)
    {
        return await LessonsWithIncludes().Where(l => l.Workout.Id == workoutId).ToListAsync();
    }

    public async Task<IEnumerable<Lesson>?> GetCurrentOrFutureLessonsByWorkoutIdAsync(Guid workoutId)
    {
        var now = DateTime.Now;
        return await LessonsWithIncludes()
            .Where(l => l.Workout.Id == workoutId)
            .ToListAsync()
            .ContinueWith(t => t.Result
                .Where(l => l.Schedule.GetNextOccurrence(now) != null)
                .AsEnumerable());
    }

    public async Task<IEnumerable<Lesson>?> GetLessonsByInstructorIdAsync(Guid instructorId)
    {
        return await LessonsWithIncludes()
            .Where(l => EF.Property<Guid>(l, "InstructorId") == instructorId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Lesson>?> GetAllLessonsAsync()
    {
        return await LessonsWithIncludes().ToListAsync();
    }

    public async Task AddLessonAsync(Lesson lesson)
    {
        // Room comes from RoomDbContext — attach it so LessonDbContext doesn't try to INSERT it
        if (DbContext.Entry(lesson.Room).State == Microsoft.EntityFrameworkCore.EntityState.Detached)
            DbContext.Attach(lesson.Room);
        await AddAsync(lesson);
    }

    public async Task UpdateLessonAsync(Lesson lesson) => await UpdateAsync(lesson);

    public async Task DeleteLessonAsync(Guid lessonId) => await DeleteAsync(lessonId);
}
