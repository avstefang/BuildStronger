using System.Globalization;
using Application.Dto;

namespace MApp.Models;

/// <summary>
/// A single scheduled class (lesson) a member can browse and book.
/// </summary>
public class GymClass
{
    private static readonly CultureInfo Dutch = CultureInfo.GetCultureInfo("nl-NL");

    public int Id { get; set; }

    /// <summary>The real lesson id from the API (the int <see cref="Id"/> is only used by the mock booking flow).</summary>
    public Guid LessonId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public int DurationMinutes { get; set; }
    public int Capacity { get; set; }
    public int BookedCount => Capacity - SpotsLeft;
    public int SpotsLeft { get; set; }

    /// <summary>True when the logged-in member already has a reservation for this occurrence.</summary>
    public bool IsBooked { get; set; }

    /// <summary>Spinning is the only class where a member picks a specific bike.</summary>
    public bool IsSpinning { get; set; }

    /// <summary>Reservations can be made up to a week in advance.</summary>
    public static readonly TimeSpan BookingWindow = TimeSpan.FromDays(7);

    public DateTime End => Start.AddMinutes(DurationMinutes);
    public string TimeRange => $"{Start:HH:mm} - {End:HH:mm}";
    public string DayLabel => Start.ToString("ddd d MMM", Dutch);
    public bool IsFull => SpotsLeft <= 0;

    /// <summary>Bookable while there's room, the member hasn't already booked it, the class is more than
    /// an hour away, and it starts within the one-week booking window.</summary>
    public bool IsBookable => !IsFull && !IsBooked
        && Start > DateTime.Now.AddHours(1)
        && Start <= DateTime.Now.Add(BookingWindow);

    public string BookButtonText => IsBooked ? "Geboekt" : "Boeken";

    public string SpotsLabel => IsFull ? "Vol" : $"{SpotsLeft} plekken vrij";

    /// <summary>Maps a lesson from the API into the shape the pages bind to.</summary>
    public static GymClass FromDto(GetLessonDto dto)
    {
        int capacity = dto.MaxCapacity > 0 ? dto.MaxCapacity : dto.Room.Capacity;
        int duration = dto.CustomDuration > 0 ? dto.CustomDuration : dto.Workout.DurationInMinutes;

        return new GymClass
        {
            LessonId = dto.Id,
            Name = dto.Workout.Name,
            Room = dto.Room.Name,
            Instructor = string.IsNullOrWhiteSpace(dto.Instructor?.FullName) ? "Nog onbekend" : dto.Instructor!.FullName,
            Start = NextOccurrence(dto.Schedule.StartDay, dto.Schedule.StartTime),
            DurationMinutes = duration,
            Capacity = capacity,
            SpotsLeft = Math.Max(0, capacity - dto.BookedCount),
            IsSpinning = dto.Workout.Name.Contains("Spinning", StringComparison.OrdinalIgnoreCase)
        };
    }

    // The schedule stores a weekday (e.g. "Monday"); resolve it to the next upcoming date at that time.
    // Public so a cached lesson can recompute its next start on load (rather than caching a stale date).
    public static DateTime NextOccurrence(string startDay, TimeOnly startTime)
    {
        if (!Enum.TryParse(startDay, ignoreCase: true, out DayOfWeek day))
            return DateTime.Today.Add(startTime.ToTimeSpan());

        int daysUntil = ((int)day - (int)DateTime.Today.DayOfWeek + 7) % 7;
        DateTime occurrence = DateTime.Today.AddDays(daysUntil).Add(startTime.ToTimeSpan());

        // If this week's slot has already passed, show next week's instead so it's always upcoming.
        if (occurrence <= DateTime.Now)
            occurrence = occurrence.AddDays(7);

        return occurrence;
    }
}
