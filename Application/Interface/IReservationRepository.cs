using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface IReservationRepository : IRepository<Reservation>
{
    Task<Reservation> GetReservationAsync(Reservation reservation);
    Task<IEnumerable<Reservation>> GetAllReservationsAsync();
    Task AddReservationAsync(Reservation reservation);
    Task UpdateReservationAsync(Reservation reservation);
    Task DeleteReservationAsync(Reservation reservation);
}
