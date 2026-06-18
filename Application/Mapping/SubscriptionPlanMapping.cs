using Application.Dto;
using Domain.Entity;

namespace Application.Mapping;

public static class SubscriptionPlanMapping
{
    public static GetSubscriptionPlanDto ToDto(this SubscriptionPlan plan) => new()
    {
        Id = plan.Id,
        Name = plan.Name,
        Price = plan.Price,
        DurationInMonths = plan.DurationInMonths,
        MonthlyCreditAmount = plan.MonthlyCreditAmount,
        PaymentMethod = plan.PaymentMethod.ToString(),
        Currency = plan.Currency.ToString()
    };
}
