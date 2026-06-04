using Domain.Enum;
using Domain.Exception;

namespace Domain.Entity;

public class Reservation
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Athlete Athlete { get; private set; }
    public Lesson Lesson { get; private set; }
    public DateTime ReservationDate { get; private set; }
    public ReservationStatus Status { get; private set; } = ReservationStatus.Waitinglist;
    public DateTime ReservedAt { get; private set; } = DateTime.Now;

    public Reservation(Athlete athlete, DateTime reservationDate, Lesson lesson)
    {
        if (DateTime.Now.AddDays(7) > reservationDate)
            throw new DomainException("A reservation cannot be made less than 7 days in advance.");

        Athlete = athlete;
        ReservationDate = reservationDate;
        Lesson = lesson;
    }

    public void CheckIn()
    {
        Status = Status == ReservationStatus.Accepted ? ReservationStatus.CheckedIn :
            throw new DomainException("Cannot check in a reservation that is not accepted.");
    }

    public void CheckOut()
    {
        Status = Status == ReservationStatus.CheckedIn ? ReservationStatus.CheckedOut :
            throw new DomainException("Cannot check out a reservation that is not checked in.");
    }

    public void Cancel()
    {
        if (DateTime.Now.AddHours(1) > ReservationDate)
            throw new DomainException("A reservation cannot be cancelled less than 1 hour before the reservation date.");

        Status = Status != ReservationStatus.Accepted ? ReservationStatus.Cancelled :
            throw new DomainException("Cannot cancel a reservation that is not accepted.");
    }

    public void Accept()
    {
        Status = Status == ReservationStatus.Waitinglist ? ReservationStatus.Accepted : 
            throw new DomainException("Cannot accept a reservation that is not on the waiting list.");
    }
}
