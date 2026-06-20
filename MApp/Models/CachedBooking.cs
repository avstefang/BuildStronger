using SQLite;

namespace MApp.Models;

/// <summary>
/// Local SQLite copy of a member booking. The start is an absolute date+time (unlike a planning
/// lesson's "next occurrence"), so it's safe to store and read back as-is.
/// </summary>
public class CachedBooking
{
    [PrimaryKey]
    public string ReservationId { get; set; } = string.Empty;

    public string ClassName { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public int DurationMinutes { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? SpotLabel { get; set; }

    public static CachedBooking FromBooking(Booking booking) => new()
    {
        ReservationId = booking.ReservationId.ToString(),
        ClassName = booking.ClassName,
        Room = booking.Room,
        Instructor = booking.Instructor,
        Start = booking.Start,
        DurationMinutes = booking.DurationMinutes,
        Status = booking.Status,
        SpotLabel = booking.SpotLabel
    };

    public Booking ToBooking() => new()
    {
        ReservationId = Guid.Parse(ReservationId),
        ClassName = ClassName,
        Room = Room,
        Instructor = Instructor,
        Start = Start,
        DurationMinutes = DurationMinutes,
        Status = Status,
        SpotLabel = SpotLabel
    };
}
