namespace Application.Dto;

public class CreateLessonDto
{
    public Guid WorkoutId { get; set; }
    public Guid RoomId { get; set; }

    /// <summary>Optional — a lesson can start "nog onbekend" (no instructor yet).</summary>
    public Guid? InstructorId { get; set; }

    public int MaxCapacity { get; set; }
    public int CustomDuration { get; set; }

    // Schedule: a weekly slot on StartDay at StartTime, recurring either a number of times
    // (RepetitionCount) or until a date (RepetitionEndDate). Set at most one of the two.
    public TimeOnly StartTime { get; set; }
    public DayOfWeek StartDay { get; set; }
    public int? RepetitionCount { get; set; }
    public DateTime? RepetitionEndDate { get; set; }
}
