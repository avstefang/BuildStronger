using System;
using System.Collections.Generic;
using System.Text;
using Application.Dto;
using Domain.Entity;

namespace Application.Interface;

public interface IReservationRepository
{
    Task<Reservation> GetReservationByNameAsync(string workoutName);
    Task<IEnumerable<Reservation>> GetAllReservationsByLessonIdAsync(Guid lessonId);
    Task<IEnumerable<Reservation>> GetAllReservationsByAthleteAsync(Athlete athlete);
    Task<IEnumerable<Reservation>> GetAllReservationsAsync();
    Task AddReservationAsync(Reservation reservation);
    Task UpdateReservationAsync(UpdateReservationDto reservationDto);
    Task DeleteReservationAsync(Guid id);
}
