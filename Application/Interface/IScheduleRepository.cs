using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface IScheduleRepository : IRepository<Schedule>
{
    Task<Schedule> GetScheduleAsync(Schedule schedule);
    Task<IEnumerable<Schedule>> GetAllSchedulesAsync();
    Task AddScheduleAsync(Schedule schedule);
    Task UpdateScheduleAsync(Schedule schedule);
    Task DeleteScheduleAsync(Schedule schedule);
}
