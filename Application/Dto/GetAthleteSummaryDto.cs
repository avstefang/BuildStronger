namespace Application.Dto;

/// <summary>
/// Slim athlete reference used where an athlete is nested inside another resource
/// (e.g. a lesson's instructor or a reservation's athlete). Full detail lives on the athlete endpoint.
/// </summary>
public class GetAthleteSummaryDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
