namespace Application.Dto;

public class UpdateLessonDto
{
    public Guid Id { get; set; }

    public Guid? InstructorId { get; set; }
    public int? MaxCapacity { get; set; }
    public int? CustomDuration { get; set; }

    // Optional schedule changes. Provide both StartTime and StartDay to update the slot;
    // set at most one of RepetitionCount / RepetitionEndDate.
    public TimeOnly? StartTime { get; set; }
    public DayOfWeek? StartDay { get; set; }
    public int? RepetitionCount { get; set; }
    public DateTime? RepetitionEndDate { get; set; }
}
