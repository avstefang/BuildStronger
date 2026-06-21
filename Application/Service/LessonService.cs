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

        // Build the schedule (weekly slot, recurring by count or until a date). The schedule's FK is on
        // its own side (LessonId), so it must be saved together with the lesson — not separately, or its
        // LessonId would be empty. Adding the lesson cascades the insert and sets the FK correctly.
        Repetition repetition = new(dto.RepetitionCount, dto.RepetitionEndDate);
        Schedule schedule = new(dto.StartTime, dto.StartDay, repetition);

        Lesson lesson = new(workout, schedule, dto.MaxCapacity, room, dto.CustomDuration);

        if (dto.InstructorId != Guid.Empty && dto.InstructorId is Guid instructorId)
        {
            Instructor instructor = await _instructorRepository.GetInstructorByIdAsync(instructorId);
            lesson.AssignInstructor(instructor);
        }

        await _lessonRepository.AddLessonAsync(lesson);
        return lesson.ToDto();
    }

    public async Task<GetLessonDto> UpdateLessonAsync(UpdateLessonDto dto)
    {
        Lesson? lesson = await _lessonRepository.GetLessonByIdAsync(dto.Id) ??
            throw new Exception($"Lesson with ID {dto.Id} not found.");

        if (dto.MaxCapacity != null)
            lesson.UpdateMaxCapacity(dto.MaxCapacity.Value);
        if (dto.CustomDuration != null)
            lesson.UpdateCustomDuration(dto.CustomDuration.Value);
        if (dto.InstructorId is Guid instructorId)
        {
            Instructor instructor = await _instructorRepository.GetInstructorByIdAsync(instructorId);
            lesson.AssignInstructor(instructor);
        }

        await _lessonRepository.UpdateLessonAsync(lesson);

        // Update the lesson's existing schedule slot in place (day/time/recurrence).
        if (dto.StartTime is TimeOnly startTime && dto.StartDay is DayOfWeek startDay)
        {
            Repetition? repetition = dto.RepetitionCount != null || dto.RepetitionEndDate != null
                ? new Repetition(dto.RepetitionCount, dto.RepetitionEndDate)
                : null;
            UpdateScheduleDto scheduleDto = new(lesson.Schedule.Id, startTime, startDay, repetition);
            await _scheduleRepository.UpdateScheduleAsync(scheduleDto);
        }

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