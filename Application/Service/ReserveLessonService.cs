using Application.Interface;
using Domain.Entity;
using Domain.Enum;
using Domain.Exception;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Service;

public class ReserveLessonService(ILessonRepository lessonRepository, IReservationRepository reservationRepository)
{
    private readonly ILessonRepository _lessonRepository = lessonRepository;
    private readonly IReservationRepository _reservationRepository = reservationRepository;

    public async Task<ReservationStatus> ReserveLesson(Athlete athlete, string lessonName, DateTime reservationDate)
    {
        Lesson lesson = await _lessonRepository.RetrieveLessonByNameAsync(lessonName);
        IEnumerable<Reservation> reservations = await _reservationRepository.RetrieveAllReservationsByLessonAsync(lesson);

        if (lesson != null)
        {
            Reservation reservation = new(athlete, reservationDate, lesson);

            if (reservations.Count() < lesson.MaxCapacity)
            {
                reservation.Accept();
            }

            return reservation.Status;
        }
        
        throw new ReservationException($"No lesson found with the name {lessonName}");
    }
}