namespace Application.Dto;

public class GetScheduleDto
{
    public Guid Id { get; init; }
    public TimeOnly StartTime { get; init; }
    public string StartDay { get; init; } = string.Empty;
    public int? RepetitionCount { get; init; }
    public DateTime? RepetitionEndDate { get; init; }
    public DateTime RegisteredAt { get; init; }
}
