namespace Application.Dto;

public class CreateLessonDto
{
    public Guid WorkoutId { get; set; }
    public Guid ScheduleId { get; set; }
    public Guid RoomId { get; set; }
    public int MaxCapacity { get; set; }
    public int CustomDuration { get; set; }
}
