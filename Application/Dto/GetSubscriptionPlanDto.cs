namespace Application.Dto;

public class GetSubscriptionPlanDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int DurationInMonths { get; init; }
    public int WeeklyCreditAmount { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
    public string Currency { get; init; } = string.Empty;
}
