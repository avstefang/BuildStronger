namespace Application.Dto;

public class UpdateLessonDto
{
    public Guid Id { get; set; }
    public Guid? ScheduleId { get; set; }
    public int? MaxCapacity { get; set; }
    public int? CustomDuration { get; set; }
}
