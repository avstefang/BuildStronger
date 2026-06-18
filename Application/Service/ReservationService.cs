using Application.Dto;
using Application.Interface;
using Application.Mapping;
using Domain.Entity;
using Domain.Enum;
using Domain.Exception;
using Domain.Value_object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Service;

public class ReservationService(ILessonRepository lessonRepository, IReservationRepository reservationRepository, IAthleteRepository athleteRepository, IEquipmentSpotRepository equipmentSpotRepository)
{
    private readonly ILessonRepository _lessonRepository = lessonRepository;
    private readonly IReservationRepository _reservationRepository = reservationRepository;
    private readonly IAthleteRepository _athleteRepository = athleteRepository;
    private readonly IEquipmentSpotRepository _equipmentSpotRepository = equipmentSpotRepository;

    public async Task<GetReservationDto> ReserveLessonAsync(AddReservationDto dto)
    {
        EmailAddress email = new(dto.Email);
        Athlete athlete = await _athleteRepository.GetAthleteByEmailAsync(email) ??
            throw new ReservationException($"No athlete found with email {dto.Email}");
        Lesson? lesson = await _lessonRepository.GetLessonByIdAsync(dto.LessonId) ??
            throw new ReservationException($"No lesson found with id {dto.LessonId}");

        IEnumerable<Reservation>? reservations = await _reservationRepository.GetAllReservationsByLessonIdAsync(lesson.Id);

        Reservation reservation = new(athlete, dto.ReservationDate, lesson);
        if (reservations?.Count() < lesson.MaxCapacity)
        {
            reservation.Accept();
        }

        if (lesson.Workout.Equipment.Count > 0)
        {
            if (dto.EquipmentSpotId == null)
            {
                throw new ReservationException($"Lesson {lesson.Id} requires an equipment spot reservation");
            }

            EquipmentSpot? equipmentSpot = await _equipmentSpotRepository.GetEquipmentSpotByIdAsync(dto.EquipmentSpotId ?? Guid.Empty) ??
                throw new ReservationException($"No equipment spot found with id {dto.EquipmentSpotId}");
            reservation.SetEquipmentSpot(equipmentSpot);
        }

        await _reservationRepository.AddReservationAsync(reservation);
        return reservation.ToDto();
    }

    public async Task<GetReservationDto> CancelReservationAsync(Guid reservationId, string email)
    {
        Reservation reservation = await _reservationRepository.GetReservationByIdAsync(reservationId) ??
            throw new ReservationException($"No reservation found with id {reservationId}");

        if (reservation.Athlete.EmailAddress.Address != email)
        {
            throw new ReservationException($"Reservation with id {reservationId} does not belong to athlete with email {email}");
        }

            reservation.Cancel();
        await _reservationRepository.UpdateReservationAsync(reservation);
        return reservation.ToDto();
    }

    public async Task<IEnumerable<GetReservationDto>?> GetLessonReservationsAsync(Guid lessonId) =>
        (await _reservationRepository.GetAllReservationsByLessonIdAsync(lessonId))?.Select(reservation => reservation.ToDto());

    public async Task<IEnumerable<GetReservationDto>?> GetAthleteReservationsAsync(string email)
    {
        EmailAddress emailAddress = new(email);
        return (await _reservationRepository.GetAllReservationsByEmailAsync(emailAddress))?.Select(reservation => reservation.ToDto());
    }

    public async Task AutoAcceptWaitlistReservationsAsync()
    {
        IEnumerable<Lesson>? lessons = await _lessonRepository.GetAllLessonsAsync();
        foreach (Lesson lesson in lessons!)
        {
            int reservationSpots = await _reservationRepository.GetReservationSpotsLeftAsync(lesson.Id);
            if (reservationSpots == 0) continue;

            IEnumerable<Reservation>? waitlistedReservations = await _reservationRepository.GetAllWaitlistedReservationsByLessonIdAsync(lesson.Id);
            if (null == waitlistedReservations || waitlistedReservations.Count() == 0) continue;

            for (int i = 0; i < reservationSpots; i++)
            {
                Reservation reservation = waitlistedReservations.ElementAt(i);
                reservation.Accept();
                await _reservationRepository.UpdateReservationAsync(reservation);
            }
        }
    }
}