using Application.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;
using Application.Dto;
using Domain.Value_object;

namespace Application.Service;

public class LessonService(ILessonRepository lessonRepository, IWorkoutRepository workoutRepository)
{
    private readonly ILessonRepository _lessonRepository = lessonRepository;
    private readonly IWorkoutRepository _workoutRepository = workoutRepository;

    public async Task<Lesson> CreateLessonAsync(string workoutName, Schedule schedule, int maxCapacity, int customDuration = 0)
    {
        Workout workout = await _workoutRepository.GetWorkoutByNameAsync(workoutName);
        Lesson lesson = new(workout, schedule, maxCapacity, customDuration);
        await _lessonRepository.AddLessonAsync(lesson);
        return lesson;
    }

    public async Task<Lesson> UpdateLessonAsync(UpdateLessonDto dto)
    {
        Lesson lesson = await _lessonRepository.GetLessonByIdAsync(dto.Id);
        if (dto.Schedule != null)
            lesson.UpdateSchedule(dto.Schedule);
        if (dto.MaxCapacity != null)
            lesson.UpdateMaxCapacity(dto.MaxCapacity.Value);
        if (dto.CustomDuration != null)
            lesson.UpdateCustomDuration(dto.CustomDuration.Value);

        await _lessonRepository.UpdateLessonAsync(dto);
        return lesson;
    }

    public async Task<Lesson> GetLessonByDateTime(string workoutName, DateTime dateTime)
    {
        IEnumerable<Lesson> lessons = await _lessonRepository.GetLessonByWorkoutNameAsync(workoutName);
        return lessons.FirstOrDefault(l => l.Schedule.StartDateTime() == dateTime) ?? 
            throw new Exception($"No lesson found at {dateTime} for workout '{workoutName}'");
    }

    public async Task<IEnumerable<Lesson>> GetLessonsByWorkoutNameAsync(string workoutName)
    {
        return await _lessonRepository.GetLessonByWorkoutNameAsync(workoutName);
    }
    public async Task DeleteLessonAsync(Guid lessonId)
    {
        await _lessonRepository.DeleteLessonAsync(lessonId);
    }
}