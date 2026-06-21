namespace Application.Dto;

public class UpdateWorkoutDto(string? name = null, string? description = null, int? durationInMinutes = null)
{
    public string? Name { get; init; } = name;
    public string? Description { get; init; } = description;
    public int? DurationInMinutes { get; init; } = durationInMinutes;
}