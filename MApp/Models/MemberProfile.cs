namespace MApp.Models;

/// <summary>
/// The logged-in member's account details. Mock shape — replace with API data later.
/// </summary>
public class MemberProfile
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string MemberSince => StartDate.ToString("d MMM yyyy");
    public string RenewsOn => EndDate.ToString("d MMM yyyy");
}
