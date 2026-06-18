namespace MApp.Models;

/// <summary>
/// A single scheduled class (lesson) a member can browse and book.
/// Mock shape — replace with data from the API later.
/// </summary>
public class GymClass
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public int DurationMinutes { get; set; }
    public int Capacity { get; set; }
    public int SpotsLeft { get; set; }

    /// <summary>Spinning is the only class where a member picks a specific bike.</summary>
    public bool IsSpinning { get; set; }

    public DateTime End => Start.AddMinutes(DurationMinutes);
    public string TimeRange => $"{Start:HH:mm} - {End:HH:mm}";
    public string DayLabel => Start.ToString("ddd d MMM");
    public bool IsFull => SpotsLeft <= 0;
    public string SpotsLabel => IsFull ? "Full" : $"{SpotsLeft} spots left";
}
