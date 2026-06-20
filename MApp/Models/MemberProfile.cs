using System.Globalization;

namespace MApp.Models;

/// <summary>
/// The logged-in member's account details. Mock shape — replace with API data later.
/// </summary>
public class MemberProfile
{
    private static readonly CultureInfo Dutch = CultureInfo.GetCultureInfo("nl-NL");

    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    /// <summary>Server filename of the profile photo (null when none was uploaded). Served by GET api/Photo/{file}.</summary>
    public string? PhotoFile { get; set; }

    public string PlanName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string MemberSince => StartDate.ToString("d MMM yyyy", Dutch);
    public string RenewsOn => EndDate.ToString("d MMM yyyy", Dutch);
    public bool IsAutoRenewalEnabled { get; set; }
}
