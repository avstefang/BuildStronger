using Application.Dto;
using Domain.Entity;

namespace Application.Mapping;

public static class ReservationMapping
{
    public static GetReservationDto ToDto(this Reservation reservation) => new()
    {
        Id = reservation.Id,
        // The Athlete navigation is intentionally ignored on the reservation context,
        // so it isn't loaded here — guard against the resulting null.
        Athlete = reservation.Athlete?.ToSummaryDto() ?? new GetAthleteSummaryDto(),
        Lesson = reservation.Lesson.ToDto(),
        ReservationDate = reservation.ReservationDate,
        Status = reservation.Status.ToString(),
        ReservedAt = reservation.ReservedAt
    };
}
