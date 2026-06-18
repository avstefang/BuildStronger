using Application.Dto;
using Domain.Entity;

namespace Application.Mapping;

public static class ReservationMapping
{
    public static GetReservationDto ToDto(this Reservation reservation) => new()
    {
        Id = reservation.Id,
        Athlete = reservation.Athlete.ToSummaryDto(),
        Lesson = reservation.Lesson.ToDto(),
        ReservationDate = reservation.ReservationDate,
        Status = reservation.Status.ToString(),
        ReservedAt = reservation.ReservedAt
    };
}
