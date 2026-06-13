using System;
using System.Collections.Generic;
using System.Text;
using Application.Dto;
using Domain.Entity;

namespace Application.Interface;

public interface ILessonRepository
{
    Task<Lesson> GetLessonByIdAsync(Guid lessonId);
    Task<IEnumerable<Lesson>> GetLessonByWorkoutNameAsync(string workoutName);
    Task<IEnumerable<Lesson>> GetAllLessonsAsync();
    Task AddLessonAsync(Lesson lesson);
    Task UpdateLessonAsync(UpdateLessonDto lessonDto);
    Task DeleteLessonAsync(Guid lessonId);
}
