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
    public int SpotsLeft { get; set; }

    /// <summary>Spinning is the only class where a member picks a specific bike.</summary>
    public bool IsSpinning { get; set; }

    public DateTime End => Start.AddMinutes(DurationMinutes);
    public string TimeRange => $"{Start:HH:mm} - {End:HH:mm}";
    public string DayLabel => Start.ToString("ddd d MMM", Dutch);
    public bool IsFull => SpotsLeft <= 0;
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
            // The lesson endpoint doesn't expose reservation counts, so we show full availability.
            SpotsLeft = capacity,
            IsSpinning = dto.Workout.Name.Contains("Spinning", StringComparison.OrdinalIgnoreCase)
        };
    }

    // The schedule stores a weekday (e.g. "Monday"); resolve it to the next upcoming date at that time.
    private static DateTime NextOccurrence(string startDay, TimeOnly startTime)
    {
        if (!Enum.TryParse(startDay, ignoreCase: true, out DayOfWeek day))
            return DateTime.Today.Add(startTime.ToTimeSpan());

        int daysUntil = ((int)day - (int)DateTime.Today.DayOfWeek + 7) % 7;
        return DateTime.Today.AddDays(daysUntil).Add(startTime.ToTimeSpan());
    }
}
