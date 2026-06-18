namespace MApp.Models;

/// <summary>
/// A reservation the logged-in member has made. Mock shape — replace with API data later.
/// </summary>
public class Booking
{
    public int Id { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public int DurationMinutes { get; set; }
    public string Status { get; set; } = "Confirmed";

    /// <summary>For spinning, the chosen bike, e.g. "Bike 14". Null for other classes.</summary>
    public string? SpotLabel { get; set; }

    public DateTime End => Start.AddMinutes(DurationMinutes);
    public string TimeRange => $"{Start:HH:mm} - {End:HH:mm}";
    public string DayLabel => Start.ToString("ddd d MMM");
    public bool IsUpcoming => Start >= DateTime.Now;
}
