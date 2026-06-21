namespace Application.Dto;

public class GetSubscriptionDto
{
    public Guid Id { get; init; }
    public GetSubscriptionPlanDto SubscriptionPlan { get; init; } = new();
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public bool AutoRenew { get; init; }
    public string Status { get; init; } = string.Empty;
}
