using Application.Dto;
using SQLite;

namespace MApp.Models;

/// <summary>
/// Local SQLite copy of a planning lesson. The weekday + time are stored raw (not the computed
/// start) so <see cref="ToGymClass"/> can recompute the next occurrence on load and never show a
/// stale/past time. Kept separate from the API DTO, like <see cref="CachedSubscriptionPlan"/>.
/// </summary>
public class CachedGymClass
{
    [PrimaryKey]
    public string LessonId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public string StartDay { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int Capacity { get; set; }
    public int BookedCount { get; set; }
    public bool IsSpinning { get; set; }

    public static CachedGymClass FromDto(GetLessonDto dto)
    {
        // Reuse GymClass.FromDto for the display/duration/capacity mapping, then keep the raw schedule.
        GymClass gymClass = GymClass.FromDto(dto);
        return new CachedGymClass
        {
            LessonId = gymClass.LessonId.ToString(),
            Name = gymClass.Name,
            Room = gymClass.Room,
            Instructor = gymClass.Instructor,
            StartDay = dto.Schedule.StartDay,
            StartTime = dto.Schedule.StartTime.ToString("HH:mm:ss"),
            DurationMinutes = gymClass.DurationMinutes,
            Capacity = gymClass.Capacity,
            BookedCount = dto.BookedCount,
            IsSpinning = gymClass.IsSpinning
        };
    }

    public GymClass ToGymClass() => new()
    {
        LessonId = Guid.Parse(LessonId),
        Name = Name,
        Room = Room,
        Instructor = Instructor,
        Start = GymClass.NextOccurrence(StartDay, TimeOnly.Parse(StartTime)),
        DurationMinutes = DurationMinutes,
        Capacity = Capacity,
        SpotsLeft = Math.Max(0, Capacity - BookedCount),
        IsSpinning = IsSpinning
    };
}
