using System;
using System.Collections.Generic;
using System.Text;
using Application.Dto;
using Domain.Entity;
using Domain.Value_object;

namespace Application.Interface;

public interface IReservationRepository
{
    Task<Reservation?> GetReservationByIdAsync(Guid id);
    Task<IEnumerable<Reservation>?> GetAllReservationsByLessonIdAsync(Guid lessonId);
    Task<IEnumerable<Reservation>?> GetAllWaitlistedReservationsByLessonIdAsync(Guid lessonId);
    Task<IEnumerable<Reservation>?> GetAllReservationsByEmailAsync(EmailAddress email);
    Task<IEnumerable<Reservation>?> GetAllReservationsAsync();
    Task<int> GetReservationSpotsLeftAsync(Guid lessonId);
    Task AddReservationAsync(Reservation reservation);
    Task UpdateReservationAsync(Reservation reservation);
    Task DeleteReservationAsync(Guid id);
}
