namespace MApp.Models;

/// <summary>
/// A subscription plan shown on the Home page. Mock shape — replace with API data later.
/// </summary>
public class SubscriptionPlanInfo
{
    public string Name { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Featured { get; set; }
}
