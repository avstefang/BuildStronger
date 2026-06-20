using Application.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;
using Application.Dto;
using Application.Mapping;
using Domain.Value_object;

namespace Application.Service;

public class LessonService(ILessonRepository lessonRepository, IWorkoutRepository workoutRepository, IRoomRepository roomRepository, IScheduleRepository scheduleRepository, IInstructorRepository instructorRepository, IReservationRepository reservationRepository)
{
    private readonly ILessonRepository _lessonRepository = lessonRepository;
    private readonly IWorkoutRepository _workoutRepository = workoutRepository;
    private readonly IRoomRepository _roomRepository = roomRepository;
    private readonly IScheduleRepository _scheduleRepository = scheduleRepository;
    private readonly IInstructorRepository _instructorRepository = instructorRepository;
    private readonly IReservationRepository _reservationRepository = reservationRepository;

    // The Instructor navigation is ignored by EF (it lives in the athlete aggregate), so populate it
    // from the lesson's InstructorId before mapping, otherwise the DTO's instructor is always null.
    private async Task PopulateInstructorsAsync(IEnumerable<Lesson> lessons)
    {
        List<Lesson> needing = lessons.Where(l => l is not null && l.InstructorId is not null).ToList();
        if (needing.Count == 0)
            return;

        Dictionary<Guid, Instructor> instructors = (await _instructorRepository.GetAllInstructorsAsync())
            .ToDictionary(i => i.Athlete.Id);

        foreach (Lesson lesson in needing)
            if (lesson.InstructorId is Guid id && instructors.TryGetValue(id, out Instructor? instructor))
                lesson.AssignInstructor(instructor);
    }

    public async Task<GetLessonDto?> CreateLessonAsync(CreateLessonDto dto)
    {
        Room? room = await _roomRepository.GetRoomByIdAsync(dto.RoomId) ??
            throw new Exception($"Room with ID {dto.RoomId} not found.");
        Workout? workout = await _workoutRepository.GetWorkoutByIdAsync(dto.WorkoutId) ??
            throw new Exception($"Workout with ID {dto.WorkoutId} not found.");
        Schedule? schedule = await _scheduleRepository.GetScheduleByIdAsync(dto.ScheduleId) ??
            throw new Exception($"Schedule with ID {dto.ScheduleId} not found.");

        Lesson lesson = new(workout, schedule, dto.MaxCapacity, room, dto.CustomDuration);
        await _lessonRepository.AddLessonAsync(lesson);
        return lesson.ToDto();
    }

    public async Task<GetLessonDto> UpdateLessonAsync(UpdateLessonDto dto)
    {
        Lesson? lesson = await _lessonRepository.GetLessonByIdAsync(dto.Id) ??
            throw new Exception($"Lesson with ID {dto.Id} not found.");

        if (dto.ScheduleId != null)
        {
            Schedule schedule = await _scheduleRepository.GetScheduleByIdAsync(dto.ScheduleId.Value)
                ?? throw new Exception($"Schedule with ID {dto.ScheduleId} not found.");
            lesson.UpdateSchedule(schedule);
        }
        if (dto.MaxCapacity != null)
            lesson.UpdateMaxCapacity(dto.MaxCapacity.Value);
        if (dto.CustomDuration != null)
            lesson.UpdateCustomDuration(dto.CustomDuration.Value);

        await _lessonRepository.UpdateLessonAsync(lesson);
        return lesson.ToDto();
    }

    public async Task<IEnumerable<GetLessonDto>?> GetLessonsByWorkoutIdAsync(Guid workoutId)
    {
        IEnumerable<Lesson>? lessons = await _lessonRepository.GetLessonsByWorkoutIdAsync(workoutId) ??
            throw new Exception($"No lessons found for workout '{workoutId}'.");
        await PopulateInstructorsAsync(lessons);
        return lessons.Where(l => l != null).Select(lesson => lesson.ToDto());
    }

    public async Task<IEnumerable<GetLessonDto>?> GetLessonsByInstructorIdAsync(Guid instructorId)
    {
        IEnumerable<Lesson>? lessons = await _lessonRepository.GetLessonsByInstructorIdAsync(instructorId) ??
            throw new Exception($"No lessons found for instructor with ID {instructorId}.");
        await PopulateInstructorsAsync(lessons);
        return lessons.Where(l => l != null).Select(lesson => lesson.ToDto());
    }

    public async Task<IEnumerable<GetLessonDto>?> GetCurrentOrFutureLessonsByWorkoutIdAsync(Guid workoutId)
    {
        IEnumerable<Lesson>? lessons = await _lessonRepository.GetCurrentOrFutureLessonsByWorkoutIdAsync(workoutId) ??
            throw new Exception($"No current or future lessons found for workout '{workoutId}'.");
        await PopulateInstructorsAsync(lessons);
        return lessons.Where(l => l != null).Select(lesson => lesson.ToDto());
    }

    public async Task DeleteLessonAsync(Guid lessonId) =>
        await _lessonRepository.DeleteLessonAsync(lessonId);

    public async Task<IEnumerable<GetLessonDto>?> GetAllLessonsAsync()
    {
        IEnumerable<Lesson>? lessons = await _lessonRepository.GetAllLessonsAsync();
        if (lessons is null)
            return null;

        await PopulateInstructorsAsync(lessons);

        // Subtract accepted bookings so the planning shows real availability instead of full capacity.
        Dictionary<Guid, int> bookedCounts = await _reservationRepository.GetAcceptedCountsByLessonAsync();
        return lessons.Where(l => l != null)
            .Select(lesson => lesson.ToDto(bookedCounts.GetValueOrDefault(lesson.Id)));
    }
}