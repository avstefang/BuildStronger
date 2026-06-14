using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class AddReservationDto
{
    public string Email { get; private set; }
    public Guid LessonId { get; private set; }
    public DateTime ReservationDate { get; private set; }
    public Guid EquipmentSpotId { get; private set; } = Guid.Empty;

    public AddReservationDto(string email, Guid lessonId, DateTime reservationDate)
    {
        Email = email;
        LessonId = lessonId;
        ReservationDate = reservationDate;
    }

    public AddReservationDto(string email, Guid lessonId, DateTime reservationDate, Guid equipmentSpotId)
    {
        Email = email;
        LessonId = lessonId;
        ReservationDate = reservationDate;
        EquipmentSpotId = equipmentSpotId;
    }
}