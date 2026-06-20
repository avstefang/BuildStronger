namespace Application.Dto;

/// <summary>A taken equipment spot for a lesson, by grid position, so the app can show it as booked.</summary>
public class GetBookedSpotDto
{
    public int RowNumber { get; init; }
    public int SpotNumber { get; init; }
}
