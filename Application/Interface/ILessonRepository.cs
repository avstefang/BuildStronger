using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface ILessonRepository : IRepository<Lesson>
{
    Task<Lesson> RetrieveLessonByNameAsync(string lessonName);
    Task<IEnumerable<Lesson>> RetrieveAllLessonsAsync();
    Task AddLessonAsync(Lesson lesson);
    Task UpdateLessonAsync(Lesson lesson);
    Task DeleteLessonAsync(Lesson lesson);
}
