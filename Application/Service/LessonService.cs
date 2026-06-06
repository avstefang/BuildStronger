using Application.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;
using Application.Dto;
using Domain.Value_object;

namespace Application.Service;

public class LessonService(ILessonRepository lessonRepository)
{
    private readonly ILessonRepository _lessonRepository = lessonRepository;

    public async Task<Lesson> CreateLessonAsync(Workout workout, Schedule schedule, int maxCapacity, int customDuration = 0)
    {
        Lesson lesson = new(workout, schedule, maxCapacity, customDuration);
        await _lessonRepository.AddLessonAsync(lesson);
        return lesson;
    }

    public async Task<Lesson> UpdateLessonAsync(UpdateLessonDto dto)
    {
        Lesson lesson = await _lessonRepository.RetrieveLessonByIdAsync(dto.Id);
        if (dto.Schedule != null)
            lesson.UpdateSchedule(dto.Schedule);
        if (dto.MaxCapacity != null)
            lesson.UpdateMaxCapacity(dto.MaxCapacity.Value);
        if (dto.CustomDuration != null)
            lesson.UpdateCustomDuration(dto.CustomDuration.Value);

        await _lessonRepository.UpdateLessonAsync(lesson);
        return lesson;
    }

    public async Task<Lesson> GetLessonByDateTime(string workoutName, DateTime dateTime)
    {
        Lesson lesson = await _lessonRepository.RetrieveLessonByWorkoutNameAsync(workoutName);
        if (lesson != null && lesson.Schedule.StartDateTime() == dateTime)
            return lesson;
        throw new Exception($"No lesson found at {dateTime} for workout '{workoutName}'");
    }
}