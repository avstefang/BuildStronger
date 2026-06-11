using Application.Interface;
using Domain.Entity;
using Domain.Enum;
using Domain.Exception;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Service;

public class ReservationService(ILessonRepository lessonRepository, IReservationRepository reservationRepository)
{
    private readonly ILessonRepository _lessonRepository = lessonRepository;
    private readonly IReservationRepository _reservationRepository = reservationRepository;

    public async Task<ReservationStatus> ReserveLessonAsync(Athlete athlete, string workoutName, DateTime reservationDate)
    {
        IEnumerable<Lesson> lessons = await _lessonRepository.GetLessonByWorkoutNameAsync(workoutName);
        Lesson? lesson = lessons.FirstOrDefault(l => l.Schedule.StartDateTime() == reservationDate);

        if (lesson != null)
        {
            IEnumerable<Reservation> reservations = await _reservationRepository.GetAllReservationsByLessonIdAsync(lesson.Id);

            Reservation reservation = new(athlete, reservationDate, lesson);

            if (reservations.Count() < lesson.MaxCapacity)
            {
                reservation.Accept();
            }

            return reservation.Status;
        }
        
        throw new ReservationException($"No lesson found with the name {workoutName}");
    }

    public async Task<bool> CancelReservationAsync(Athlete athlete, string workoutName, DateTime reservationDate)
    {
        IEnumerable<Lesson> lessons = await _lessonRepository.GetLessonByWorkoutNameAsync(workoutName);
        Lesson? lesson = lessons.FirstOrDefault(l => l.Schedule.StartDateTime() == reservationDate);
        if (lesson != null)
        {
            IEnumerable<Reservation> reservations = await _reservationRepository.GetAllReservationsByLessonIdAsync(lesson.Id);
            Reservation? reservation = reservations.FirstOrDefault(r => r.Athlete.Id == athlete.Id && r.ReservationDate == reservationDate);
            if (reservation != null)
            {
                reservation.Cancel();
                return true;
            }
            throw new ReservationException($"No reservation found for athlete {athlete.FullName} on {reservationDate} for lesson {workoutName}");
        }
        throw new ReservationException($"No lesson found with the name {workoutName}");
    }

    public async Task<IEnumerable<Reservation>> GetLessonReservationsAsync(Guid lessonId)
    {
        IEnumerable<Reservation> reservations = await _reservationRepository.GetAllReservationsByLessonIdAsync(lessonId);
        if (reservations != null)
        {
            return reservations;
        }
        throw new ReservationException($"No reservations found for lesson {lessonId}");
    }

    public async Task<IEnumerable<Reservation>> GetAthleteReservationsAsync(Athlete athlete)
    {
        return await _reservationRepository.GetAllReservationsByAthleteAsync(athlete);
    }

    public async Task<List<Reservation>?> GetWaitlistReservationsAsync(Guid lessonId)
    {
        IEnumerable<Reservation> reservations = await _reservationRepository.GetAllReservationsByLessonIdAsync(lessonId);
        return reservations.Where(r => r.Status == ReservationStatus.Waitinglist).ToList();
    }

    public async Task<Reservation?> GetAthleteWaitlistReservationsAsync(Athlete athlete, Lesson lesson)
    {
        IEnumerable<Reservation> reservations = await _reservationRepository.GetAllReservationsByAthleteAsync(athlete);
        return reservations.FirstOrDefault(r => r.Status == ReservationStatus.Waitinglist && r.Lesson.Id == lesson.Id);
    }
}