using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class CancelReservationDto(string email, Guid reservationId)
{
    public string Email { get; private set; } = email;
    public Guid ReservationId { get; private set; } = reservationId;
}