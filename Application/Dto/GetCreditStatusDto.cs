namespace Application.Dto;

/// <summary>The booking credits for the member's current period, for the account page slider.</summary>
public class GetCreditStatusDto
{
    public int Total { get; init; }
    public int Used { get; init; }
    public int Remaining { get; init; }

    /// <summary>True for very large allowances (unlimited plans), so the UI shows "Onbeperkt" instead of a bar.</summary>
    public bool Unlimited { get; init; }
}
