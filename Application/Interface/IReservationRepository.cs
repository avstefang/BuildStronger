using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface IReservationRepository : IRepository<Reservation>
{
    Task<Reservation> RetrieveReservationByNameAsync(string reservationName);
    Task<IEnumerable<Reservation>> RetrieveAllReservationsByLessonAsync(Lesson lesson);
    Task<IEnumerable<Reservation>> RetrieveAllReservationsAsync();
    Task AddReservationAsync(Reservation reservation);
    Task UpdateReservationAsync(Reservation reservation);
    Task DeleteReservationAsync(Reservation reservation);
}
