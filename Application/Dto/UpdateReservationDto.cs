using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class UpdateReservationDto(Guid Id, Guid AthleteId, Guid LessonId, DateTime ReservationDate)
{
    public Guid Id { get; private set; } = Id;
    public Guid AthleteId { get; private set; } = AthleteId;
    public Guid LessonId { get; private set; } = LessonId;
    public DateTime ReservationDate { get; private set; } = ReservationDate;
}