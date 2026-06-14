using Application.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;
using Application.Dto;
using Domain.Value_object;

namespace Application.Service;

public class LessonService(ILessonRepository lessonRepository, IWorkoutRepository workoutRepository, IRoomRepository roomRepository)
{
    private readonly ILessonRepository _lessonRepository = lessonRepository;
    private readonly IWorkoutRepository _workoutRepository = workoutRepository;
    private readonly IRoomRepository _roomRepository = roomRepository;

    public async Task<Lesson?> CreateLessonAsync(CreateLessonDto dto)
    {
        Room? room = await _roomRepository.GetRoomByIdAsync(dto.RoomId) ??
            throw new Exception($"Room with ID {dto.RoomId} not found.");
        Workout? workout = await _workoutRepository.GetWorkoutByIdAsync(dto.WorkoutId) ??
            throw new Exception($"Workout with ID {dto.WorkoutId} not found.");

        Lesson lesson = new(workout, dto.Schedule, dto.MaxCapacity, room, dto.CustomDuration);
        await _lessonRepository.AddLessonAsync(lesson);
        return lesson;
    }

    public async Task<Lesson> UpdateLessonAsync(UpdateLessonDto dto)
    {
        Lesson? lesson = await _lessonRepository.GetLessonByIdAsync(dto.Id) ??
            throw new Exception($"Lesson with ID {dto.Id} not found.");

        if (dto.Schedule != null)
            lesson.UpdateSchedule(dto.Schedule);
        if (dto.MaxCapacity != null)
            lesson.UpdateMaxCapacity(dto.MaxCapacity.Value);
        if (dto.CustomDuration != null)
            lesson.UpdateCustomDuration(dto.CustomDuration.Value);

        await _lessonRepository.UpdateLessonAsync(dto);
        return lesson;
    }

    public async Task<IEnumerable<Lesson>?> GetLessonsByWorkoutIdAsync(Guid workoutId)
    {
        IEnumerable<Lesson>? lessons = await _lessonRepository.GetLessonsByWorkoutIdAsync(workoutId) ??
            throw new Exception($"No lessons found for workout '{workoutId}'.");
        return lessons;
    }

    public async Task<IEnumerable<Lesson>?> GetLessonsByInstructorIdAsync(Guid instructorId)
    {
        IEnumerable<Lesson>? lessons = await _lessonRepository.GetLessonsByInstructorIdAsync(instructorId) ??
            throw new Exception($"No lessons found for instructor with ID {instructorId}.");
        return lessons;
    }

    public async Task<IEnumerable<Lesson>?> GetCurrentOrFutureLessonsByWorkoutIdAsync(Guid workoutId)
    {
        IEnumerable<Lesson>? lessons = await _lessonRepository.GetCurrentOrFutureLessonsByWorkoutIdAsync(workoutId) ??
            throw new Exception($"No current or future lessons found for workout '{workoutId}'.");
        return lessons;
    }

    public async Task DeleteLessonAsync(Guid lessonId) =>
        await _lessonRepository.DeleteLessonAsync(lessonId);

    public async Task<IEnumerable<Lesson>?> GetAllLessonsAsync() =>
        await _lessonRepository.GetAllLessonsAsync();
}