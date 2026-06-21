namespace Application.Dto;

public class CreateWorkoutDto(string name, string description, int durationInMinutes, List<Guid>? equipmentIds = null)
{
    public string Name { get; init; } = name;
    public string Description { get; init; } = description;
    public int DurationInMinutes { get; init; } = durationInMinutes;
    public List<Guid> EquipmentIds { get; init; } = equipmentIds ?? [];
}