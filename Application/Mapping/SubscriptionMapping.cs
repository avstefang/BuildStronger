using Application.Dto;
using Domain.Entity;

namespace Application.Mapping;

public static class SubscriptionMapping
{
    public static GetSubscriptionDto ToDto(this Subscription subscription) => new()
    {
        Id = subscription.Id,
        SubscriptionPlan = subscription.SubscriptionPlan.ToDto(),
        StartDate = subscription.StartDate,
        EndDate = subscription.GetEndDate(),
        AutoRenew = subscription.AutoRenew,
        Status = subscription.Status.ToString()
    };
}
