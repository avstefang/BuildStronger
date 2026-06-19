using Application.Dto;
using SQLite;

namespace MApp.Models;

/// <summary>
/// Local SQLite copy of a subscription plan. Kept separate from
/// <c>GetSubscriptionPlanDto</c> (which lives in the shared Application project)
/// so the API contract stays free of persistence attributes.
/// </summary>
public class CachedSubscriptionPlan
{
    // The API's Guid is stored as text so SQLite can use it as a stable primary key.
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationInMonths { get; set; }
    public int MonthlyCreditAmount { get; set; }
    // Stored in SQLite as one comma-separated string, e.g. "iDEAL,Wero".
    public string PaymentMethods { get; set; } = string.Empty;

    public string Currency { get; set; } = string.Empty;

    // Not persisted — a split/join view over PaymentMethods, the way Address
    // splits and joins its parts on a separator. [Ignore] keeps sqlite-net from
    // trying to create a column for it (it can't store a list).
    [Ignore]
    public IReadOnlyList<string> PaymentMethodList
    {
        get => string.IsNullOrWhiteSpace(PaymentMethods)
            ? Array.Empty<string>()
            : PaymentMethods.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        set => PaymentMethods = value is null ? string.Empty : string.Join(',', value);
    }

    /// <summary>Maps an API plan to its local cache shape (the comma-separated string is kept as-is).</summary>
    public static CachedSubscriptionPlan FromDto(GetSubscriptionPlanDto dto) => new()
    {
        Id = dto.Id.ToString(),
        Name = dto.Name,
        Price = dto.Price,
        DurationInMonths = dto.DurationInMonths,
        MonthlyCreditAmount = dto.MonthlyCreditAmount,
        PaymentMethods = dto.PaymentMethod,
        Currency = dto.Currency
    };
}
