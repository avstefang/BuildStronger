using Application.Dto;
using Application.Interface;
using Domain.Entity;
using Domain.Value_object;
using Infrastructure.Context_model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class ScheduleRepository(LessonDbContext dbContext) : Repository<Schedule, Guid>(dbContext), IScheduleRepository
{
    public async Task<Schedule> GetScheduleByIdAsync(Guid scheduleId)
    {
        return await DbContext.Set<Schedule>().FindAsync(scheduleId)
            ?? throw new InvalidOperationException($"Schedule with ID {scheduleId} not found.");
    }

    public async Task<IEnumerable<Schedule>> GetAllSchedulesAsync()
    {
        return await DbContext.Set<Schedule>().ToListAsync();
    }

    public async Task AddScheduleAsync(Schedule schedule) => await AddAsync(schedule);

    public async Task UpdateScheduleAsync(UpdateScheduleDto dto)
    {
        Schedule schedule = await GetScheduleByIdAsync(dto.Id);
        schedule.ChangeStartTime(dto.StartTime);
        schedule.ChangeStartDay(dto.StartDay);
        if (dto.Repetition != null)
            schedule.ChangeRepetition(dto.Repetition);
        await UpdateAsync(schedule);
    }

    public async Task DeleteScheduleAsync(Guid scheduleId) => await DeleteAsync(scheduleId);
}
