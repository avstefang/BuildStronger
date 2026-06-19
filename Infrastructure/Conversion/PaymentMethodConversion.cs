using Domain.Enum;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Conversion;

/// <summary>
/// Stores a plan's offered payment methods as one comma-separated string in the
/// <c>paymentMethod</c> column (e.g. "Wero,PayPal") and splits it back into a list
/// on read. Shared by every DbContext that maps <see cref="SubscriptionPlan"/> so the
/// conversion can't drift between them. TrimEntries tolerates legacy values that were
/// stored with spaces after the comma, like "Wero, PayPal".
/// </summary>
public static class PaymentMethodConversion
{
    private const char Separator = ',';

    public static readonly ValueConverter<IReadOnlyList<PaymentMethod>, string> Converter = new(
        methods => string.Join(Separator, methods.Select(m => m.ToString())),
        value => value
            .Split(Separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(s => Enum.Parse<PaymentMethod>(s, ignoreCase: true))
            .ToList());

    // A collection property needs an explicit comparer so EF can detect changes correctly.
    public static readonly ValueComparer<IReadOnlyList<PaymentMethod>> Comparer = new(
        (a, b) => (a ?? new List<PaymentMethod>()).SequenceEqual(b ?? new List<PaymentMethod>()),
        v => v.Aggregate(0, (hash, m) => HashCode.Combine(hash, m.GetHashCode())),
        v => v.ToList());
}
