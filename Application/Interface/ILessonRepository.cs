using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface ILessonRepository : IRepository<Lesson>
{
    Task<Lesson> GetLessonAsync(Lesson lesson);
    Task<IEnumerable<Lesson>> GetAllLessonsAsync();
    Task AddLessonAsync(Lesson lesson);
    Task UpdateLessonAsync(Lesson lesson);
    Task DeleteLessonAsync(Lesson lesson);
}
