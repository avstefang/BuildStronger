namespace Application.Dto;

public class GetReservationDto
{
    public Guid Id { get; init; }
    public GetAthleteSummaryDto Athlete { get; init; } = new();
    public GetLessonDto Lesson { get; init; } = new();
    public DateTime ReservationDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime ReservedAt { get; init; }
}
