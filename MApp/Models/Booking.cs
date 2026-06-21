using System.Globalization;
using Application.Dto;

namespace MApp.Models;

/// <summary>
/// A reservation the logged-in member has made.
/// </summary>
public class Booking
{
    private static readonly CultureInfo Dutch = CultureInfo.GetCultureInfo("nl-NL");

    public int Id { get; set; }

    /// <summary>The real reservation id from the API, used to cancel.</summary>
    public Guid ReservationId { get; set; }

    public string ClassName { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public int DurationMinutes { get; set; }
    public string Status { get; set; } = "Bevestigd";

    /// <summary>For spinning, the chosen bike, e.g. "Fiets 14". Null for other classes.</summary>
    public string? SpotLabel { get; set; }

    public DateTime End => Start.AddMinutes(DurationMinutes);
    public string TimeRange => $"{Start:HH:mm} - {End:HH:mm}";
    public string DayLabel => Start.ToString("ddd d MMM", Dutch);
    public bool IsUpcoming => Start >= DateTime.Now;

    /// <summary>A booking can only be cancelled until one hour before it starts.</summary>
    public bool CanCancel => Start > DateTime.Now.AddHours(1);

    /// <summary>Maps a reservation from the API into the shape the Bookings page binds to.</summary>
    public static Booking FromDto(GetReservationDto dto)
    {
        int duration = dto.Lesson.CustomDuration > 0 ? dto.Lesson.CustomDuration : dto.Lesson.Workout.DurationInMinutes;

        return new Booking
        {
            ReservationId = dto.Id,
            ClassName = dto.Lesson.Workout.Name,
            Room = dto.Lesson.Room.Name,
            Instructor = string.IsNullOrWhiteSpace(dto.Lesson.Instructor?.FullName) ? "Nog onbekend" : dto.Lesson.Instructor!.FullName,
            // reservationDate is stored as a DATE (no time), so combine it with the lesson's start time.
            Start = dto.ReservationDate.Date.Add(dto.Lesson.Schedule.StartTime.ToTimeSpan()),
            DurationMinutes = duration,
            Status = TranslateStatus(dto.Status)
        };
    }

    // The API returns the ReservationStatus enum name; show a Dutch label.
    private static string TranslateStatus(string status) => status switch
    {
        "Accepted" => "Bevestigd",
        "Waitinglist" => "Wachtlijst",
        "Cancelled" => "Geannuleerd",
        "CheckedIn" => "Ingecheckt",
        "CheckedOut" => "Uitgecheckt",
        _ => status
    };
}
