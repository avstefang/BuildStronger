using System;
using System.Collections.Generic;
using System.Text;
using Application.Dto;
using Domain.Entity;

namespace Application.Interface;

public interface ILessonRepository
{
    Task<Lesson?> GetLessonByIdAsync(Guid lessonId);
    Task<IEnumerable<Lesson>?> GetLessonsByWorkoutIdAsync(Guid workoutId);
    Task<IEnumerable<Lesson>?> GetCurrentOrFutureLessonsByWorkoutIdAsync(Guid workoutId);
    Task<IEnumerable<Lesson>?> GetLessonsByInstructorIdAsync(Guid instructorId);
    Task<IEnumerable<Lesson>?> GetAllLessonsAsync();
    Task AddLessonAsync(Lesson lesson);
    Task UpdateLessonAsync(UpdateLessonDto lessonDto);
    Task DeleteLessonAsync(Guid lessonId);
}
