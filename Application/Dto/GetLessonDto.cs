namespace Application.Dto;

public class GetLessonDto
{
    public Guid Id { get; init; }
    public GetWorkoutDto Workout { get; init; } = new();
    public GetScheduleDto Schedule { get; init; } = new();
    public GetRoomDto Room { get; init; } = new();
    public GetAthleteSummaryDto? Instructor { get; init; }
    public int CustomDuration { get; init; }
    public int MaxCapacity { get; init; }
    public TimeOnly EndTime { get; init; }
}
