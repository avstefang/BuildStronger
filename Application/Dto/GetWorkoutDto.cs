namespace Application.Dto;

public class GetWorkoutDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int DurationInMinutes { get; init; }
    public List<GetEquipmentDto> Equipment { get; init; } = [];
}
