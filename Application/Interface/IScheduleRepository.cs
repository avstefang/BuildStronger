using System;
using System.Collections.Generic;
using System.Text;
using Application.Dto;
using Domain.Entity;

namespace Application.Interface;

public interface IScheduleRepository : IRepository<Schedule, Guid>
{
    Task<Schedule> GetScheduleByIdAsync(Guid scheduleId);
    Task<IEnumerable<Schedule>> GetAllSchedulesAsync();
    Task AddScheduleAsync(Schedule schedule);
    Task UpdateScheduleAsync(UpdateScheduleDto scheduleDto);
    Task DeleteScheduleAsync(Guid scheduleId);
}
